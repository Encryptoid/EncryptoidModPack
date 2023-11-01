using EmpyrionModdingFramework.Database;
using EmpyrionModdingFramework.Teleport;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tetherport
{
    internal class TetherportTargetHelper
    {
        private static readonly ConcurrentDictionary<string, WarpLocations> _cachedWarpLocations = new ConcurrentDictionary<string, WarpLocations>();

        private readonly IDatabaseManager _dbManager;
        private readonly Action<string> _log;

        private const string PortalOffsetFileName = "portal.offsets.tether";

        public TetherportTargetHelper(IDatabaseManager dbManager, Action<string> logFunc)
        {
            _dbManager = dbManager;
            _log = logFunc;
        }

        public Vector3<float> GetCoordinates(PortalRecord record)
        {
            var offsetRecord = _dbManager
                .LoadRecords<PortalOffsetRecord>(PortalOffsetFileName)
                ?.FirstOrDefault(x => record.Name == x.Name);

            if (offsetRecord == null)
            {
                _cachedWarpLocations.TryRemove(record.Name, out var _);
                return new Vector3<float>(record.PosX, record.PosY, record.PosZ);
            }

            var points = _cachedWarpLocations.AddOrUpdate(record.Name,
                name =>
                {
                    return new WarpLocations(
                        new Vector3<float>(record.PosX, record.PosY, record.PosZ),
                        offsetRecord.Radius,
                        offsetRecord.Count);
                },
                (name, current) =>
                {
                    if (current.IsValid(record, offsetRecord))
                        return current;

                    return new WarpLocations(
                        new Vector3<float>(record.PosX, record.PosY, record.PosZ),
                        offsetRecord.Radius,
                        offsetRecord.Count);
                });

            var next = points.NextLocation;
#if DEBUG
            _log($"Tetherport Coordinate Override - Name: {offsetRecord.Name}, Radius: {offsetRecord.Radius}, Count: {offsetRecord.Count}, Next: {next}");
            _log($"Tetherport Coordinate Override - Calculated Locations: {string.Join(", ", points.Locations)}");
#endif

            return next;
        }

        public readonly struct Vector3<T> : IEquatable<Vector3<T>>
        {
            public T X { get; }
            public T Y { get; }
            public T Z { get; }

            public Vector3(T x, T y, T z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override string ToString()
            {
                return $"Vec<{typeof(T).Name}>[X: {X}, Y: {Y}, Z: {Z}]";
            }

            public override bool Equals(object other)
            {
                return other is Vector3<T> vector && Equals(vector);
            }

            public bool Equals(Vector3<T> other)
            {
                return EqualityComparer<T>.Default.Equals(X, other.X) &&
                       EqualityComparer<T>.Default.Equals(Y, other.Y) &&
                       EqualityComparer<T>.Default.Equals(Z, other.Z);
            }

            public override int GetHashCode()
            {
                int hashCode = -307843816;
                hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(X);
                hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Y);
                hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Z);
                return hashCode;
            }

            public static bool operator ==(Vector3<T> left, Vector3<T> right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Vector3<T> left, Vector3<T> right)
            {
                return !(left == right);
            }
        }

        private class WarpLocations
        {
            private readonly Vector3<float> _center;
            private readonly int _radius;
            private readonly List<Vector3<float>> _locations = new List<Vector3<float>>();
            private int _index = 0;

            public IReadOnlyList<Vector3<float>> Locations => _locations;

            public Vector3<float> NextLocation
            {
                get
                {
                    if (++_index >= _locations.Count)
                        _index = 0;

                    return _locations[_index];
                }
            }

            public WarpLocations(Vector3<float> center, int radius, int count)
            {
                _center = center;
                _radius = radius;

                var arcLength = (2 * Math.PI * radius) / count;
                var angle = arcLength / radius;
                var currentAngle = 0d;

                for (var i = 0; i < count; i++)
                {
                    var x = center.X + radius * Math.Cos(currentAngle);
                    var z = center.Z + radius * Math.Sin(currentAngle);

                    _locations.Add(new Vector3<float>((float)x, center.Y, (float)z));

                    currentAngle += angle;
                }
            }

            public bool IsValid(PortalRecord portal, PortalOffsetRecord offset)
            {
                return _center.X == portal.PosX && _center.Y == portal.PosY && _center.Z == portal.PosZ &&
                    _radius == offset.Radius && Locations.Count == offset.Count;
            }
        }
    }
}
