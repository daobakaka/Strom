using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssetToImageConverter : MonoBehaviour
{
    public Texture2D textureToConvert; // ������� Inspector �����н������ļ���ק������ֶ�

    // �� Unity �༭�����Ҽ��˵������һ��ѡ�����ִ��ת������
    [ContextMenu("Convert Asset to Image")]
    private void ConvertAssetToImage()
    {
        if (textureToConvert != null)
        {
            // ������ת��Ϊ����������
            byte[] imageBytes = textureToConvert.EncodeToTGA();

            // �����������ݱ���ΪͼƬ�ļ�
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
