using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeBuilder
{
    public class CreateNodeWindow : ScriptableObject, ISearchWindowProvider
    {
        private Texture2D icon;
        private bool isSourceParent;
        private EditorUtility.ScriptTemplate[] scriptFileAssets;
        private NodeView source;
        private BehaviourTreeView treeView;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"))
            };

            List<string> groups = new List<string>();

            foreach (var item in GetListNode())
            {
                var entryTitle = item.Path.Split('/');
                var groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]),i+1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }
                Action invoke = () => CreateNode(item.TypeNode, context);
                tree.Add(new SearchTreeEntry(new GUIContent(ObjectNames.NicifyVariableName(entryTitle.Last()), icon))
                { level = entryTitle.Length, userData = invoke});
            }
            
            Action createActionScript = () => CreateScript(context);
            tree.Add(new SearchTreeEntry(new GUIContent("New Script", icon)) 
                { level = 1, userData = createActionScript });
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var invoke = (Action)searchTreeEntry.userData;
            invoke();
            return true;
        }

        public void Initialise(BehaviourTreeView treeView, NodeView source, bool isSourceParent)
        {
            this.treeView = treeView;
            this.source = source;
            this.isSourceParent = isSourceParent;

            icon = new Texture2D(1, 1);
            icon.SetPixel(0, 0, Color.clear);
            icon.Apply();
        }

        public void CreateNode(Type type, SearchWindowContext context)
        {
            var editorWindow = BehaviourTreeEditorWindow.Instance;

            var windowMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = editorWindow.treeView.contentViewContainer.WorldToLocal(windowMousePosition);
            var nodeOffset = new Vector2(-75, -20);
            var nodePosition = graphMousePosition + nodeOffset;

            // #TODO: Unify this with CreatePendingScriptNode
            NodeView createdNode;
            if (source != null)
            {
                if (isSourceParent)
                    createdNode = treeView.CreateNode(type, nodePosition, source);
                else
                    createdNode = treeView.CreateNodeWithChild(type, nodePosition, source);
            }
            else
            {
                createdNode = treeView.CreateNode(type, nodePosition, null);
            }

            treeView.SelectNode(createdNode);
        }

        public void CreateScript(SearchWindowContext context)
        {
            var editorWindow = BehaviourTreeEditorWindow.Instance;

            var windowMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = editorWindow.treeView.contentViewContainer.WorldToLocal(windowMousePosition);
            var nodeOffset = new Vector2(-75, -20);
            var nodePosition = graphMousePosition + nodeOffset;
            EditorWindow.GetWindow<SearchWindow>().Close();
            EditorUtility.CreateNewScript(source, isSourceParent, nodePosition);
        }

        private List<ListNodeSearchData> GetListNode()
        {
            var types = TypeCache.GetTypesDerivedFrom<Node>();
            var listView = new List<ListNodeSearchData>();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<AddNodeMenuAttribute>();
                if (attribute != null)
                {
                    var path =  attribute.MenuName != "" ? attribute.MenuName + "/" + type.Name : "Other/" + type.Name;
                    listView.Add(new ListNodeSearchData(type, path));
                }
            }
            
            listView.Sort((a, b) =>
            {
                var split1 = a.Path.Split("/");
                var split2 = b.Path.Split("/");

                if (split1[0] == "Other") return 1;
                if (split2[0] == "Other") return -1;

                for (int i = 0; i < split1.Length; i++)
                {
                    if (i > split2.Length)
                    {
                        return 1;
                    }

                    int value = split1[i].CompareTo(split2[i]);
                    if (value != 0)
                    {
                        if (split1.Length != split2.Length && (i == split1.Length - 1 || i == split2.Length - 1))
                        {
                            return split1.Length < split2.Length ? 1 : -1;
                        }

                        return value;
                    }
                }

                return 0;
            });

            return listView;
        }

        public static void Show(Vector2 mousePosition, NodeView source, bool isSourceParent = false)
        {
            var screenPoint = GUIUtility.GUIToScreenPoint(mousePosition);
            var searchWindowProvider = CreateInstance<CreateNodeWindow>();
            searchWindowProvider.Initialise(BehaviourTreeEditorWindow.Instance.treeView, source, isSourceParent);
            var windowContext = new SearchWindowContext(screenPoint, 240, 320);
            SearchWindow.Open(windowContext, searchWindowProvider);
        }
        
    }
    
    public class ListNodeSearchData
    {
        public string Path;
        public Type TypeNode;

        public ListNodeSearchData(Type type, string path)
        {
            TypeNode = type;
            Path = path;
        }
    }
}
