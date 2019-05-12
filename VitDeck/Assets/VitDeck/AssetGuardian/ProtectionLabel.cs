using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VitDeck.Utilities;

namespace VitDeck.AssetGuardian
{
    /// <summary>
    /// アセットを保護する/しないを管理する。
    /// </summary>
    public static class ProtectionLabel
    {
        private const string readonlyLabel = "VitDeck.ReadOnly";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // HideFlagsの設定はInitializeOnLoadより後でないと一部のアセットで失敗するので遅延させる。
            EditorApplication.update += DelayedInitialize;
        }

        private static void DelayedInitialize()
        {
            var assets = EnumerateAllAttachedAssets();
            foreach (var asset in assets)
            {
                SetEditable(asset, false);
            }
            EditorApplication.update -= DelayedInitialize;
        }

        /// <summary>
        /// アセット/ディレクトリを保護対象にする。
        /// </summary>
        /// <remarks>
        /// 対象がディレクトリの場合、全ての子を保護対象にします。
        /// </remarks>
        /// <param name="path">対象のパス</param>
        public static void Attach(string path)
        {
            var assets = AssetUtility.EnumerateAssets(path);
            foreach (var asset in assets)
            {
                Attach(asset);
            }
        }

        /// <summary>
        /// アセット/ディレクトリを保護対象から外す。
        /// </summary>
        /// <remarks>
        /// 対象がディレクトリの場合、全ての子が保護対象から外れます。
        /// </remarks>
        /// <param name="path">対象のパス</param>
        public static void Detach(string path)
        {
            var assets = AssetUtility.EnumerateAssets(path);
            foreach (var asset in assets)
            {
                Detach(asset);
            }
        }

        public static void Attach(UnityEngine.Object asset)
        {
            if (IsProtected(asset))
                return;

            var labels = AssetDatabase.GetLabels(asset);
            labels = labels.Concat(new string[] { readonlyLabel }).ToArray();
            AssetDatabase.SetLabels(asset, labels);
            SetEditable(asset, false);
        }

        public static void Detach(UnityEngine.Object asset)
        {
            if (!IsProtected(asset))
                return;

            var labels = AssetDatabase.GetLabels(asset);
            labels = labels.Where(label => label != readonlyLabel).ToArray();
            AssetDatabase.SetLabels(asset, labels);
            SetEditable(asset, true);
        }

        private static void SetEditable(UnityEngine.Object asset, bool editable)
        {
            if (editable)
                asset.hideFlags &= ~UnityEngine.HideFlags.NotEditable;
            else
                asset.hideFlags |= UnityEngine.HideFlags.NotEditable;

        }

        /// <summary>
        /// パスが保護対象に含まれているかどうか判定する。
        /// </summary>
        /// <param name="assetPath">判定するアセットのパス</param>
        /// <returns>保護対象であればtrue、そうでなければfalse。</returns>
        public static bool IsAttachedTo(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            return IsProtected(asset);
        }

        public static IEnumerable<UnityEngine.Object> EnumerateAllAttachedAssets()
        {
            var assetGUIDs = AssetDatabase.FindAssets("l:" + readonlyLabel);
            foreach (var guid in assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                yield return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }
        }

        private static bool IsProtected(UnityEngine.Object asset)
        {
            var labels = AssetDatabase.GetLabels(asset);

            foreach (var label in labels)
            {
                if (label == readonlyLabel)
                    return true;
            }

            return false;
        }
    }
}