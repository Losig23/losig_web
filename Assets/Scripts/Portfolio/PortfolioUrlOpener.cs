using System;
using System.IO;
using UnityEngine;

public static class PortfolioUrlOpener
{
    public static void OpenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.LogWarning("No portfolio URL is configured for this interactable.");
            return;
        }

        Application.OpenURL(url);
    }

    public static void OpenStreamingAsset(string fileName, string webFallbackUrl = "")
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            OpenUrl(webFallbackUrl);
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        string streamingAssetUrl = CombineUrl(Application.streamingAssetsPath, Uri.EscapeDataString(fileName));
        OpenUrl(string.IsNullOrWhiteSpace(webFallbackUrl) ? streamingAssetUrl : webFallbackUrl);
#else
        string assetPath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(assetPath))
        {
            Application.OpenURL(new Uri(assetPath).AbsoluteUri);
            return;
        }

        if (!string.IsNullOrWhiteSpace(webFallbackUrl))
        {
            OpenUrl(webFallbackUrl);
            return;
        }

        Debug.LogWarning($"StreamingAssets file was not found at: {assetPath}");
#endif
    }

    private static string CombineUrl(string baseUrl, string fileName)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            return fileName;
        }

        return $"{baseUrl.TrimEnd('/')}/{fileName}";
    }
}
