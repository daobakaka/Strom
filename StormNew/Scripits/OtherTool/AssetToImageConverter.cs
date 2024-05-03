using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssetToImageConverter : MonoBehaviour
{
    public Texture2D textureToConvert; // 你可以在 Inspector 窗口中将纹理文件拖拽到这个字段

    // 在 Unity 编辑器中右键菜单中添加一个选项，用于执行转换操作
    [ContextMenu("Convert Asset to Image")]
    private void ConvertAssetToImage()
    {
        if (textureToConvert != null)
        {
            // 将纹理转换为二进制数据
            byte[] imageBytes = textureToConvert.EncodeToTGA();

            // 将二进制数据保存为图片文件
            string outputPath = "D:/unitystrom/Assets/Other/Scripits/OtherTool/OutPutImage" + "/ConvertedImage.tga";
            System.IO.File.WriteAllBytes(outputPath, imageBytes);

            Debug.Log("Asset converted and saved as image: " + outputPath);
        }
        else
        {
            Debug.LogError("Texture to convert is not assigned.");
        }
    }
}
