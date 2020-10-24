
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum Alignment
    {
        North,
        South,
        East,
        West
    }

    [Serializable]
public class Entrance
    {
        private const int PossibleTiles = 10;
        private const int SpaceFromEntrance = 3;
        [SerializeField] 
        public Transform Transform;

        public Vector2 Location => Transform.position;

        [SerializeField] public Alignment Alignment;

        private readonly Dictionary<Vector2, Lawman> _coveringLawmen = new Dictionary<Vector2, Lawman>();




        public Entrance()
        {
            // Generate all possible tiles next to door


        }

        public void GenerateTiles()
        {
            if (_coveringLawmen.Count > 0)
                return;

            for (var i = -(PossibleTiles / 2); i < (PossibleTiles / 2); i++)
            {
                Vector2 tile;
                switch (Alignment)
                {
                    case Alignment.North:
                        tile = new Vector2(Location.x + i, Location.y + SpaceFromEntrance );
                        break;
                    case Alignment.South:
                        tile = new Vector2(Location.x + i, Location.y - SpaceFromEntrance);
                        break;
                    case Alignment.East:
                        tile = new Vector2(Location.x - SpaceFromEntrance, Location.y + i);
                        break;
                    case Alignment.West:
                        tile = new Vector2(Location.x + SpaceFromEntrance, Location.y + i);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

              //  Debug.Log($"Count: {_coveringLawmen.Count}. Adding tile: {tile} to entrance: {Location}");
                _coveringLawmen.Add(tile, null);
            }
        }


        public int GetTileCount()
        {
            return _coveringLawmen.Count(p => p.Value != null);
        }
        public Vector2 AddLawman(Lawman lawman)
        {
            var emptySpot = _coveringLawmen.FirstOrDefault(l => l.Value == null);
            if (emptySpot.Equals(default(KeyValuePair<Vector2, Lawman>)))
            {
                return lawman.transform.position;
            }
            _coveringLawmen[emptySpot.Key] = lawman;
            return emptySpot.Key;
        }
    public Vector2 TryAddLawman(Lawman lawman, out bool success)
        {
            var emptySpot = _coveringLawmen.FirstOrDefault(l => l.Value == null);
            if (emptySpot.Equals(default(KeyValuePair<Vector2, Lawman>)))
            {
                success = false;
                return Vector2.zero;
            }

            _coveringLawmen[emptySpot.Key] = lawman;
            success = true;
            return emptySpot.Key;
        }
        public bool RemoveLawman(Lawman lawman)
        {
            var spot = _coveringLawmen.FirstOrDefault(l => l.Value == lawman);
            if (spot.Equals(default(KeyValuePair<Vector2, Lawman>)))
                return false;

            _coveringLawmen[spot.Key] = null;
            return true;
        }
}
