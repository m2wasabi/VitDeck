﻿using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

namespace VitDeck.Validator
{
    /// <summary>
    /// アセット名の使用禁止文字を検出するルール
    /// </summary>
    public class AssetNamingRule : BaseRule
    {
        private readonly string patternMatch;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">ルール名</param>
        /// <param name="assetName">アセット名</param>
        public AssetNamingRule(string name, string patternMatch = "^[\x21-\x7e]+$") : base(name)
        {
            this.patternMatch = patternMatch;
        }

        protected override void Logic(ValidationTarget target)
        {
            var paths = target.GetAllAssetPaths();
            
            foreach (var path in paths)
            {
                var filename = Path.GetFileName(path);
                if (!Regex.IsMatch(filename, patternMatch))
                {
                    var reference = AssetDatabase.LoadMainAssetAtPath(path);                    
                    var message = string.Format("アセット名({0})に使用禁止文字が含まれています。(使用可能文字の範囲={1})", filename, patternMatch);                    
                    AddIssue(new Issue(reference, IssueLevel.Error, message, string.Empty));
                }
            }
        }
    }
}