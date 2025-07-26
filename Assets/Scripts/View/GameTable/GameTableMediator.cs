using System;
using System.Collections.Generic;
using Controllers;
using Services.Input;
using TMPro;
using UI.Game.CardPreviews;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View.GameTable
{
    public class GameTableMediator
    {
        private const string CardViewSortingLayerNameDefault = "Default";
        private const string CardViewSortingLayerNameDraggable = IDraggable.DraggableViewSortingLayerName;

        public event Action OnCardDragRollback;

        private readonly GridManager _gridManager;
        private readonly CardViewPool _cardViewPool;
        private readonly ConnectionFactory _connectionFactory;

        private readonly VisualGridFactory _visualGridFactory;
        private readonly IEnvironmentFactory _environmentFactory;

        private readonly Dictionary<Vector2Int, GameObject> _generatedChunks = new();

        private bool _isConstructed;

        private bool _isDragging;
        private Vector3 _originalPosition;
        private CardView _originalCardView;
        private CardView _createdCardView;
        private CardView _draggableCardView;

        private IDraggable _draggablePort;
        private Transform _connectionsContainer;

        private Camera _camera;

        private Transform _environmentContainer;

        private Mesh _mesh;
        private GameObject _buildGrid;

        private float _environmentSeed;

        public GameTableMediator(
            GridManager gridManager,
            CardViewPool cardViewPool,
            ConnectionFactory connectionFactory,
            VisualGridFactory visualGridFactory,
            IEnvironmentFactory environmentFactory
        )
        {
            _gridManager = gridManager;
            _cardViewPool = cardViewPool;
            _connectionFactory = connectionFactory;
            _environmentFactory = environmentFactory;
            _visualGridFactory = visualGridFactory;
        }

        public void ConstructGameTable(Camera camera)
        {
            _camera = camera;

            _gridManager.Construct(World.DefaultGameObjectInjectionWorld.EntityManager);
            _cardViewPool.Construct();
            _connectionsContainer = _connectionFactory.CreateConnectionsContainer();

            (_mesh, _buildGrid) = _visualGridFactory.Construct();
            DrawVisualGrid();

            _environmentFactory.LoadAssets();
            _environmentContainer = new GameObject("Environment Container").transform;
            _environmentSeed = Random.Range(10_000_000, 99_999_999);
            UpdateEnvironmentAround(camera.transform.position);

            _isConstructed = true;

            int[,] map =
            {
                { 32100, 32100, 32100 },
                { 31210, 31211, 31211 },
                { 30100, 31100, 31100 },
                { 32100, 32100, 32100 }
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var card = _cardViewPool.GetCardView(map[i, j]);
                    // SnapCardToGridByWorldPosition(card, new Vector2Int(5, 7));
                    SnapCardToGridByWorldPosition(card, new Vector3(0, 0, 0));
                    card.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateEnvironmentAround(Vector3 cameraPosition)
        {
            int chunkSize = Constants.EnvironmentSettings.ChunkSize;
            int activeChunkRange = Constants.EnvironmentSettings.ActiveChunkRange;

            Vector2Int chunkCoords = new(
                (int)Mathf.Floor(cameraPosition.x / chunkSize),
                (int)Mathf.Floor(cameraPosition.y / chunkSize)
            );

            HashSet<Vector2Int> requiredChunks = new();

            for (int dx = -activeChunkRange; dx <= activeChunkRange; dx++)
            {
                for (int dy = -activeChunkRange; dy <= activeChunkRange; dy++)
                {
                    Vector2Int chunkCoord = chunkCoords + new Vector2Int(dx, dy);
                    requiredChunks.Add(chunkCoord);

                    if (!_generatedChunks.TryGetValue(chunkCoord, out GameObject chunk))
                    {
                        chunk = GenerateChunk(chunkCoord);
                        _generatedChunks.Add(chunkCoord, chunk);
                    }

                    chunk.SetActive(true);
                }
            }

            foreach (var (coord, chunk) in _generatedChunks)
            {
                if (!requiredChunks.Contains(coord))
                    chunk.SetActive(false);
            }
        }

        private GameObject GenerateChunk(Vector2Int chunkCoord)
        {
            GameObject chunkRoot = new($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
            chunkRoot.transform.SetParent(_environmentContainer.transform);

            int chunkSize = Constants.EnvironmentSettings.ChunkSize;
            int cellStep = Constants.EnvironmentSettings.CellStep;
            float zoom = Constants.EnvironmentSettings.Zoom;

            int baseX = chunkCoord.x * chunkSize;
            int baseY = chunkCoord.y * chunkSize;

            for (int x = 0; x < chunkSize; x += cellStep)
            {
                for (int y = 0; y < chunkSize; y += cellStep)
                {
                    int gridX = baseX + x;
                    int gridY = baseY + y;

                    Vector3 worldPosition = _gridManager.CellToWorld(new Vector2Int(gridX, gridY));
                    float noise = Mathf.PerlinNoise(
                        (gridX + _environmentSeed) / zoom,
                        (gridY + _environmentSeed) / zoom
                    );

                    _environmentFactory.CreateEnvironmentObject(noise, worldPosition, chunkRoot.transform);
                }
            }

            return chunkRoot;
        }

        private void DrawVisualGrid()
        {
            _mesh.Clear();

            float zoom = Mathf.InverseLerp(Constants.CameraSettings.ZoomMin, Constants.CameraSettings.ZoomMax,
                _camera.orthographicSize);
            
            float camWidth = _camera.orthographicSize * _camera.aspect * 2f;
            float camHeight = _camera.orthographicSize * 2f;
            
            Vector3 camPos = _camera.transform.position;

            float left = camPos.x - camWidth / 2;
            float right = camPos.x + camWidth / 2;
            float bottom = camPos.y - camHeight / 2;
            float top = camPos.y + camHeight / 2;

            var gridLevels = new List<VisualGridLevel>
            {
                new(1f, 0.00f, 0.1f, 0.01f, 0.1f),
                new (3f,  0.1f, 0.25f, 0.1f, 0.25f),
                new (45f, 0.25f, 0.75f, 0.15f, 0.65f),
                new (180f, 0.75f, 1.00f, 0.15f, 1.0f)
            };

            List<Vector3> vertices = new();
            List<int> indices = new();
            List<Color> colors = new();

            foreach (var level in gridLevels)
            {
                float alpha = level.GetAlpha(zoom);
                if (alpha <= 0f)
                    continue;

                float cellSize = level.CellSize;

                float startX = Mathf.Floor(left / cellSize) * cellSize;
                float endX = Mathf.Ceil(right / cellSize) * cellSize;
                float startY = Mathf.Floor(bottom / cellSize) * cellSize;
                float endY = Mathf.Ceil(top / cellSize) * cellSize;

                Color lineColor = new Color(1, 1, 1, alpha); 

                for (float x = startX; x <= endX; x += cellSize)
                {
                    vertices.Add(new Vector3(x, startY));
                    vertices.Add(new Vector3(x, endY));
                    colors.Add(lineColor);
                    colors.Add(lineColor);
                    indices.Add(vertices.Count - 2);
                    indices.Add(vertices.Count - 1);
                }

                for (float y = startY; y <= endY; y += cellSize)
                {
                    vertices.Add(new Vector3(startX, y));
                    vertices.Add(new Vector3(endX, y));
                    colors.Add(lineColor);
                    colors.Add(lineColor);
                    indices.Add(vertices.Count - 2);
                    indices.Add(vertices.Count - 1);
                }
            }

            _mesh.SetVertices(vertices);
            _mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            _mesh.SetColors(colors);
        }

        public void SnapCardToGridByWorldPosition(CardView cardView, Vector3 position)
        {
            AssertConstructed();

            var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(
                position,
                cardView.PlaceableReference,
                out var needRelocate
            );

            if (placedPosition.HasValue)
            {
                cardView.transform.SetPositionAndRotation(placedPosition.Value, Quaternion.identity);
                // TODO: Relocate
            }
        }

        public void GridVisibility(TMP_Text buttonText)
        {
            if (_buildGrid.activeSelf)
            {
                _buildGrid.SetActive(false);
                buttonText.text = "Off";
            }
            else
            {
                _buildGrid.SetActive(true);
                buttonText.text = "On";
            }
        }

        public void OnCameraChanged(Transform cameraPosition)
        {
            DrawVisualGrid();
            UpdateEnvironmentAround(cameraPosition.position);
        }

        public void HandleStartDraw(DragContext context)
        {
            if (_draggablePort != null)
                return;

            IDraggable draggable = context.Draggable;

            if (draggable is not PortView)
                return;

            _connectionFactory.CreateConnectionView(_connectionsContainer);

            _draggablePort = draggable;
        }

        public void HandleDraw(DragContext context)
        {
            if (_draggablePort == null) return;
            // pathfinding
        }

        public void HandleStopDraw(DragContext context)
        {
            _draggablePort = null;
            // pathfinding
        }

        public void HandleStartDrag(CardDragContext context)
        {
            _isDragging = true;

            if (context.Draggable is CardView cardView)
            {
                _originalPosition = cardView.transform.position;
                _originalCardView = cardView;
                _draggableCardView = _originalCardView;

                cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
                cardView.transform.SetPositionAndRotation(NormalizeWorldPosition(context), Quaternion.identity);
            }
            else
            {
                _originalPosition = Vector3.zero;
                _originalCardView = null;
                _draggableCardView = null;
            }
        }

        public void HandleDrag(CardDragContext context)
        {
            if (!_isDragging || !_draggableCardView) return;

            _draggableCardView.transform.SetPositionAndRotation(NormalizeWorldPosition(context), Quaternion.identity);
        }

        public void HandleChangeToPreview(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardView)
                HideCardView(_draggableCardView);

            _draggableCardView = null;
        }


        public void HandleChangeToView(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_originalCardView != null)
            {
                _draggableCardView = _originalCardView;
            }
            else
            {
                if (_createdCardView == null)
                {
                    if (context.Draggable is not UICardPreview preview)
                        throw new Exception("Draggable is not UICardPreview or CardView");

                    _createdCardView = _cardViewPool.GetCardView(preview.CardId, true);
                    _createdCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
                }

                _draggableCardView = _createdCardView;
            }

            ShowCardView(_draggableCardView);
            _draggableCardView.transform.SetPositionAndRotation(context.WorldMousePosition, Quaternion.identity);
        }

        public void HandleStopDrag(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardView)
            {
                _draggableCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(
                    NormalizeWorldPosition(context),
                    _draggableCardView.PlaceableReference,
                    out var needRelocate
                );

                if (!placedPosition.HasValue)
                {
                    OnCardDragRollback?.Invoke();
                    return;
                }

                _draggableCardView.transform.SetPositionAndRotation(placedPosition.Value, Quaternion.identity);
                _draggableCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                ShowCardView(_draggableCardView);

                _originalCardView = null;
                _createdCardView = null;
            }
            else if (_originalCardView)
            {
                _gridManager.Release(_originalCardView.PlaceableReference);
            }

            ClearDragState();
        }

        public void HandleRollback(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_originalCardView)
            {
                _originalCardView.transform.SetPositionAndRotation(_originalPosition, Quaternion.identity);
                _originalCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                ShowCardView(_originalCardView);
                _originalCardView = null;
            }

            ClearDragState();
        }

        private void ClearDragState()
        {
            if (_originalCardView)
            {
                ShowCardView(_originalCardView);
                _cardViewPool.ReturnCardView(_originalCardView);
            }

            if (_createdCardView)
            {
                ShowCardView(_createdCardView);
                _cardViewPool.ReturnCardView(_createdCardView);
            }

            _isDragging = false;
            _originalPosition = Vector3.zero;
            _originalCardView = null;
            _createdCardView = null;
            _draggableCardView = null;
        }

        private Vector3 NormalizeWorldPosition(CardDragContext context)
        {
            return _originalCardView != null
                ? context.WorldMousePosition - (Vector3)context.LocalHitPoint
                : context.WorldMousePosition;
        }

        private static void HideCardView(CardView cardView)
        {
            foreach (var renderer in cardView.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        private static void ShowCardView(CardView cardView)
        {
            foreach (var renderer in cardView.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }
        }

        public void DestructGameTable()
        {
            _isConstructed = false;

            _cardViewPool.Destruct();
            _gridManager.Destruct();
        }

        private void AssertConstructed()
        {
            if (!_isConstructed)
                throw new Exception("GridView is not constructed");
        }
    }
}