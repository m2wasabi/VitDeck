using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VitDeck.Utilities;

namespace VitDeck.TemplateLoader.GUI
{
    /// <summary>
    /// テンプレートから作成機能のGUI
    /// </summary>
    public class TemplateLoaderWindow : EditorWindow
    {
        const string prefix = "VitDeck/";
        private static TemplateLoaderWindow window;
        private static string[] templateFolders = { };
        private static string[] templateOptions = { };
        private static int popupIndex = 0;
        private static Vector2 licenceScroll;
        private static TemplateProperty templateProperty;
        private static List<Message> messages = new List<Message>();
        private static Dictionary<string, string> replaceStringList = new Dictionary<string, string>();

        [MenuItem(prefix + "Load Template", priority = 100)]
        static void Ooen()
        {
            window = GetWindow<TemplateLoaderWindow>(false, "VitDeck");
            Init();
            window.Show();
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            templateFolders = TemplateLoader.GetTemplateFolders();
            templateOptions = TemplateLoader.GetTemplateNames(templateFolders);
            popupIndex = 0;
            licenceScroll = new Vector2();
            messages = new List<Message>();
            if (templateFolders.Length > 0)
            {
                templateProperty = TemplateLoader.GetTemplateProperty(templateFolders[popupIndex]);
                replaceStringList = CreateReplaceStringList(templateProperty.replaceList);
            }
            else
            {
                messages.Add(new Message("テンプレートがありません。", MessageType.Warning));
            }
        }

        private static Dictionary<string, string> CreateReplaceStringList(ReplacementDefinition[] replaceList)
        {
            var list = new Dictionary<string, string>();
            if (replaceList != null)
            {
                foreach (var def in replaceList)
                {
                    list.Add(def.ID, "");
                }
            }
            return list;
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.LabelField("Template Loader");
            popupIndex = EditorGUILayout.Popup("Template:", popupIndex, templateOptions);
            if (UnityEngine.GUI.changed)
            {
                templateProperty = TemplateLoader.GetTemplateProperty(templateFolders[popupIndex]);
                replaceStringList = CreateReplaceStringList(templateProperty.replaceList);
                licenceScroll = new Vector2();
                messages = new List<Message>();
            }
            if (templateProperty != null)
            {
                //Template Property
                EditorGUILayout.LabelField("", UnityEngine.GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Description:", templateProperty.description);
                EditorGUILayout.LabelField("Developer:", templateProperty.developer);
                if (!string.IsNullOrEmpty(templateProperty.developerUrl))
                    CustomGUILayout.URLButton("Open developer website", templateProperty.developerUrl);
                if (templateProperty.lisenseFile)
                {
                    licenceScroll = EditorGUILayout.BeginScrollView(licenceScroll);
                    GUILayout.TextArea(templateProperty.lisenseFile.text);
                    EditorGUILayout.EndScrollView();
                }
                //Replace List
                EditorGUILayout.LabelField("", UnityEngine.GUI.skin.horizontalSlider);
                if (templateProperty.replaceList != null)
                {
                    foreach (var replace in templateProperty.replaceList)
                    {
                        replaceStringList[replace.ID] = EditorGUILayout.TextField(replace.label, replaceStringList[replace.ID]);
                    }
                }
                EditorGUI.BeginDisabledGroup(!CheckAllReplaceFieldFilled(replaceStringList));
                if (GUILayout.Button("作成"))
                {
                    messages = new List<Message>();
                    var folderName = templateFolders[popupIndex];
                    var templateName = templateOptions[popupIndex];
                    if (TemplateLoader.Load(folderName, replaceStringList))
                    {
                        messages.Add(new Message(string.Format("テンプレート`{0}`をコピーしました。", templateName), MessageType.Info));
                    }
                    else
                    {
                        messages.Add(new Message(string.Format("テンプレート`{0}`のコピーに失敗しました。", templateName), MessageType.Error));
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            foreach (var msg in messages)
            {
                EditorGUILayout.HelpBox(msg.message, msg.type, true);
            }
        }

        private static bool CheckAllReplaceFieldFilled(Dictionary<string, string> replaceStringList)
        {
            foreach (var str in replaceStringList.Values)
            {
                if (string.IsNullOrEmpty(str))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// HelpBoxに表示するメッセージ
        /// </summary>
        internal class Message
        {
            public Message(string _message, MessageType _type)
            {
                message = _message;
                type = _type;
            }
            public string message;
            public MessageType type;
        }

    }
}
