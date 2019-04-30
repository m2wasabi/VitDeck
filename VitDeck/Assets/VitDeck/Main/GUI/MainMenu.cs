using UnityEditor;

namespace VitDeck.Main.GUI
{
    /// <summary>
    /// メインメニューをUnityのメニューバーに表示します。
    /// </summary>
    public static class MainMenu
    {
        const string prefix = "VitDeck/";

        [MenuItem(prefix + "Settings", priority = 0)]
        static void ProjectSettings()
        {
            // グローバルな設定画面を開く
        }

        [MenuItem(prefix + "Load Template", priority = 100)]
        static void LoadTemplate()
        {
            // TemplateLoaderを開く
        }

        [MenuItem(prefix + "Check Booth", priority = 101)]
        static void CheckBooth()
        {
            // ブースチェッカーを開く
        }

        [MenuItem(prefix + "Export Booth", priority = 102)]
        static void ExportBooth()
        {
            // ブースのエクスポートプロセスを実行する
        }

        [MenuItem(prefix + "Info", priority = 200)]
        static void Information()
        {
            InfoWindow.ShowWindow();
        }
    }
}