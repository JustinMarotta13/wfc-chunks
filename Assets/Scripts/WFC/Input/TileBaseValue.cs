using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class TileBaseValue : IValue<TileBase>
    {
        TileBase _tilebase;
        public TileBaseValue(TileBase tilebase)
        {
            _tilebase = tilebase;
        }

        public TileBase Value { get => _tilebase; }

        public bool Equals(IValue<TileBase> other)
        {
            return _tilebase == other.Value;
        }

        public bool Equals(IValue<TileBase> x, IValue<TileBase> y)
        {
            return x == y;
        }

        public int GetHashCode(IValue<TileBase> obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _tilebase.GetHashCode();
        }

    }
}

