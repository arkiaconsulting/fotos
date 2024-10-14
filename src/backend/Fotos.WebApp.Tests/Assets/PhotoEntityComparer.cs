using Fotos.WebApp.Types;

namespace Fotos.WebApp.Tests.Assets;

internal sealed class PhotoEntityComparer : IEqualityComparer<PhotoEntity>
{
    bool IEqualityComparer<PhotoEntity>.Equals(PhotoEntity? x, PhotoEntity? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return x.Id == y.Id;
    }

    int IEqualityComparer<PhotoEntity>.GetHashCode(PhotoEntity obj) => obj.Id.GetHashCode();
}