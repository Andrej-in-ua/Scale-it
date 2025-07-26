using UnityEngine;

namespace View.GameTable
{
    public readonly struct VisualGridLevel
    {
        public readonly float CellSize;      
        
        private readonly float _fullStart;     
        private readonly float _fullEnd;       
        private readonly float _blendRange;    
        private readonly float _maxAlpha;      

        public VisualGridLevel(
            float cellSize,
            float fullStart,
            float fullEnd,
            float blendRange,
            float maxAlpha
        )
        {
            CellSize    = cellSize;
            _fullStart   = fullStart;
            _fullEnd     = fullEnd;
            _blendRange  = blendRange;
            _maxAlpha    = maxAlpha;
        }

        public float GetAlpha(float zoom)
        {
            float blendStart = _fullStart - _blendRange;
            float blendEnd   = _fullEnd   + _blendRange;
            float alpha;

            if (zoom < blendStart || zoom > blendEnd)
                alpha = 0f;
            else if (zoom >= _fullStart && zoom <= _fullEnd)
                alpha = 1f;
            else if (zoom < _fullStart)
                alpha = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(blendStart, _fullStart, zoom));
            else
                alpha = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(_fullEnd, blendEnd, zoom));

            return alpha * _maxAlpha;
        }
    }
}