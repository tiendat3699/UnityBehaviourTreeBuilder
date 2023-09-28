using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviourTreeBuilder
{
    public class NewScriptWindow : OdinEditorWindow
    {
        public enum NodeType
        {
            Action,
            Composite,
            Decorator
        }

        private Vector2 _nodePosition;
        private NodeView _source;
        private bool _isSourceParent;
        private bool _inEditor;
        private BehaviourTreeProjectSettings _setting;
        
        [SerializeField] [Required] private string _scriptName;
        [SerializeField] [Required, FolderPath] private string _savePath;
        [SerializeField] [EnumToggleButtons] private NodeType _nodeType;
        private string _scriptPath;

        private void OnBecameVisible()
        {
            if (_setting == null) _setting = BehaviourTreeProjectSettings.GetOrCreateSettings();
            _savePath = _setting.newNodePath;
        }

        [MenuItem("Behaviour Tree/Create New Node Script", false, 3)]
        public static void OpenWindow()
        {
            var window = GetWindow<NewScriptWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(400, 160);
            window.maximized = false;
            window.maxSize = new(400, 160);
            window.minSize = new(400, 160);
            window.ShowModal();
        }

        [Button(ButtonSizes.Large)]
        [GUIColor(0, 1f, 0)]
        private void CreateNewScript()
        {
            if(String.IsNullOrEmpty(_scriptName)) return;
            var scriptName = _scriptName;
            var template = new EditorUtility.ScriptTemplate();
            switch (_nodeType)
            {
                case NodeType.Action:
                    template.TemplateFile = GetScriptTemplate(0);
                    template.SubFolder = "Actions";

                    break;
                case NodeType.Composite:
                    template.TemplateFile = GetScriptTemplate(1);
                    template.SubFolder = "Composite";
                    break;
                case NodeType.Decorator:
                    template.TemplateFile = GetScriptTemplate(2);
                    template.SubFolder = "Decorator";
                    break;
            }

            var newNodePath = _savePath;
            if (AssetDatabase.IsValidFolder(newNodePath))
            {
                var path = $"{newNodePath}/{template.SubFolder}";
                if (path[path.Length - 1] == '/')
                    path = path.Substring(0, path.Length - 1);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                
                var contentTemplate = template.TemplateFile.text;
                contentTemplate = contentTemplate.Replace("#SCRIPTNAME#", scriptName);
                if (_setting.autoGenarateNameSpace)
                {
                    string nameSpace = _setting.rootNamespace + path
                        .Replace("Assets/", "")
                        .Replace("/",".")
                        .Replace(" ", "");
                    contentTemplate = contentTemplate
                        .Replace($"#NAMESPACEBEGIN#", $"namespace {nameSpace} \n{{")
                        .Replace("#NAMESPACEEND#", "}");
                }

                _scriptPath = $"{path}/{scriptName}.cs";

                if (!File.Exists(_scriptPath))
                {
                    File.WriteAllText(_scriptPath, contentTemplate);

                    // TODO: There must be a better way to survive domain reloads after script compiling than this
                    if (_inEditor)
                    {
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.pendingCreate = true;
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.scriptName = scriptName;
                        BehaviourTreeEditorWindow.Instance.pendingScriptCreate.nodePosition = _nodePosition;
                        if (_source != null)
                        {
                            BehaviourTreeEditorWindow.Instance.pendingScriptCreate.sourceGuid = _source.node.guid;
                            BehaviourTreeEditorWindow.Instance.pendingScriptCreate.isSourceParent = _isSourceParent;
                        }
                    }
                    AssetDatabase.Refresh();
                    // Also flash the folder yellow to highlight it
                    var obj = AssetDatabase.LoadAssetAtPath(_scriptPath, typeof(Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                    _setting.newNodePath = _savePath;
                    Close();
                    // EditorApplication.delayCall += DelayCall;
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("Error!!!",
                        $"Script with that name already exists:{_scriptPath}", "OK");
                    // Close();
                }
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("Error!!!",
                    $"Invalid folder path:{newNodePath}",
                    "OK");
            }
        }

        private void DelayCall()
        {
            var obj = AssetDatabase.LoadAssetAtPath(_scriptPath, typeof(Object));
            Selection.activeObject = obj;
            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);
        }

        public void CreateScript(NodeView source, bool isSourceParent, Vector2 nodePosition)
        {
            _source = source;
            _isSourceParent = isSourceParent;
            _nodePosition = nodePosition;
            _inEditor = true;
            OpenWindow();
        }

        private TextAsset GetScriptTemplate(int type)
        {
            var config =
                AssetDatabase.LoadAssetAtPath<DefaultConfig>(
                    "Packages/com.tiendat3699.behaviourtreebuilder/Editor/Config/DefaultConfig.asset");

            switch (type)
            {
                case 0:
                    if (_setting.scriptTemplateActionNode) return _setting.scriptTemplateActionNode;
                    return _setting.autoGenarateNameSpace ? config.scriptTemplateActionNodeNamespace : config.scriptTemplateActionNodeNoNamespace;
                case 1:
                    if (_setting.scriptTemplateCompositeNode) return _setting.scriptTemplateCompositeNode;
                    return _setting.autoGenarateNameSpace ? config.scriptTemplateCompositeNodeNamespace : config.scriptTemplateCompositeNodeNoNamespace;
                case 2:
                    if (_setting.scriptTemplateDecoratorNode) return _setting.scriptTemplateDecoratorNode;
                    return _setting.autoGenarateNameSpace ? config.scriptTemplateDecoratorNodeNamespace : config.scriptTemplateDecoratorNodeNoNamespace;
            }

            Debug.LogError("Unhandled script template type:" + type);
            return null;
        }
    }
}