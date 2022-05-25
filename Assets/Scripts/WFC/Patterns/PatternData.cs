using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class PatternData
    {
        private Pattern _pattern;
        private int _frequency = 1;
        private float _frequencyRelative;
        private float _frequencyRelativelog2;

        public float FrequencyRelative { get => _frequencyRelative; }
        public Pattern Pattern { get => _pattern;}
        public float Frequencylog2 { get => _frequencyRelativelog2; }

        public PatternData(Pattern pattern)
        {
            _pattern = pattern;       
        }
        public void AddToFrequency()
        {
            _frequency++;
        }

        public void CalculateRelativeFrequency(int total)
        {
            _frequencyRelative = (float)_frequency / total;
            _frequencyRelativelog2 = Mathf.Log(_frequencyRelative, 2);
        }

        internal bool CompareGrid(Direction dir, PatternData patternData)
        {
            return _pattern.ComparePatternToAnotherPattern(dir, patternData.Pattern);
        }
    }
}

