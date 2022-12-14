using System.Security.Cryptography;
using TpsEbReader.DataStructures;

namespace TpsEbReader;

internal class Point : SystemEntity
{
    public Point()
    {
        Sources = new List<Tuple<Point, string, PointReferenceType>>();
        Destinations = new List<Tuple<Point, string, PointReferenceType>>();
    }

    public List<Tuple<Point, string, PointReferenceType>> Sources { get; set; }
    public List<Tuple<Point, string, PointReferenceType>> Destinations { get; set; }

}

public enum PointReferenceType
{
    P2PConnection,
    AMConnection
}