using Unity.Entities;
using Unity.Collections;
using System.Text;

public static class BlobAssetUtilities
{
    public static string ConvertBlobAssetReferenceToString(BlobAssetReference<PlayerID> nameBlobAssetReference)
    {
        if (!nameBlobAssetReference.IsCreated)
        {
            return string.Empty;
        }
        ref var charArray = ref nameBlobAssetReference.Value.Name;
        char[] chars = new char[charArray.Length];

        for (int i = 0; i < charArray.Length; i++)
        {
            chars[i] = charArray[i];
        }

        return new string(chars);
    }
}
