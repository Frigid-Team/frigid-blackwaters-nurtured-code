#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class AnimatorBodyTool : FrigidEditorWindow
    {
        private static Dictionary<Type, AnimatorToolPropertyDrawer> propertyDrawersToPropertyTypes;

        [SerializeField]
        private AnimatorBody body;

        [SerializeField]
        private Vector2 windowScrollPos;

        [SerializeField]
        private int editPropertyIndex;
        [SerializeField]
        private int editAnimationIndex;
        [SerializeField]
        private int editFrameIndex;
        [SerializeField]
        private int editOrientationIndex;

        [SerializeField]
        private int editCategoryIndex;
        [SerializeField]
        private Vector2 editFieldsScrollPos;

        [SerializeField]
        private int previewNumberTiles;
        [SerializeField]
        private bool isDraggingPreview;
        [SerializeField]
        private Vector2 previewOffset;

        [SerializeField]
        private int copyPropertyIndex;
        [SerializeField]
        private int copyAnimationIndex;
        [SerializeField]
        private int copyFrameIndex;
        [SerializeField]
        private int copyOrientationIndex;

        [SerializeField]
        private Vector2 cellsScrollPos;
        [SerializeField]
        private int framePageIndex;

        [SerializeField]
        private Vector2 orientationBarScrollPos;
        [SerializeField]
        private bool isDraggingOrientation;
        [SerializeField]
        private int numShiftDivisions;

        private AnimatorBodyToolConfig config;

        static AnimatorBodyTool()
        {
            propertyDrawersToPropertyTypes = new Dictionary<Type, AnimatorToolPropertyDrawer>();
            TypeCache.TypeCollection propertyDrawerTypes = TypeCache.GetTypesWithAttribute(typeof(CustomAnimatorToolPropertyDrawerAttribute));
            foreach (Type propertyEditorType in propertyDrawerTypes)
            {
                if (propertyEditorType.IsSubclassOf(typeof(AnimatorToolPropertyDrawer)))
                {
                    CustomAnimatorToolPropertyDrawerAttribute customPropertyDrawerAttribute = (CustomAnimatorToolPropertyDrawerAttribute)propertyEditorType.GetCustomAttribute(typeof(CustomAnimatorToolPropertyDrawerAttribute));
                    if (!propertyDrawersToPropertyTypes.ContainsKey(customPropertyDrawerAttribute.PropertyType))
                    {
                        if (customPropertyDrawerAttribute.PropertyType.IsSubclassOf(typeof(AnimatorProperty)))
                        {
                            propertyDrawersToPropertyTypes.Add(customPropertyDrawerAttribute.PropertyType, (AnimatorToolPropertyDrawer)Activator.CreateInstance(propertyEditorType));
                        }
                    }
                }
            }
        }

        protected override string Title
        {
            get
            {
                return "Animator Body";
            }
        }

        protected override void Opened()
        {
            base.Opened();

            if (!AssetDatabaseUpdater.TryFindAsset<AnimatorBodyToolConfig>(out this.config))
            {
                Debug.LogError("Could not find a AnimatorBodyToolConfig asset!");
                return;
            }

            this.isDraggingPreview = false;
            this.isDraggingOrientation = false;

            this.copyPropertyIndex = -1;
            this.copyAnimationIndex = -1;
            this.copyFrameIndex = -1;
            this.copyOrientationIndex = -1;

            this.framePageIndex = 0;

            Undo.undoRedoPerformed += this.HandleUndoRedo;
        }

        protected override void Closed()
        {
            base.Closed();
            Undo.undoRedoPerformed -= this.HandleUndoRedo;
        }

        protected override void Draw()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (!Selection.activeGameObject)
                {
                    GUILayout.Label("No GameObject Selected", UtilityStyles.WordWrapAndCenter(EditorStyles.label));
                    return;
                }

                AnimatorBody newBody = Selection.activeGameObject.GetComponent<AnimatorBody>();
                if (this.body == null || this.body != newBody)
                {
                    this.body = newBody;
                    if (this.body != null)
                    {
                        this.UpdateEditPropertyIndex(this.editPropertyIndex);
                        this.UpdateEditAnimationIndex(this.editAnimationIndex);
                    }
                }

                if (this.body == null)
                {
                    GUILayout.Label("No AnimatorBody Selected", UtilityStyles.WordWrapAndCenter(EditorStyles.label));
                    if (GUILayout.Button("Create Animator Body", EditorStyles.toolbarButton))
                    {
                        AnimatorBody.CreateBody(Selection.activeGameObject);
                    }
                    return;
                }

                if (PrefabUtility.IsPartOfAnyPrefab(this.body))
                {
                    GUILayout.Label("Open The Prefab To Edit", UtilityStyles.WordWrapAndCenter(EditorStyles.label));
                    return;
                }
            }

            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.windowScrollPos))
            {
                this.windowScrollPos = scrollViewScope.scrollPosition;
                this.DrawAnimationSelectionArea();
                this.DrawPreviewArea();
                this.DrawTimelineArea();
            }
        }

        [MenuItem(FrigidPaths.MenuItem.Window + "Animator Body")]
        private static void ShowAnimatorBodyTool()
        {
            Show<AnimatorBodyTool>();
        }

        private AnimatorToolPropertyDrawer CreatePropertyDrawerForProperty(AnimatorProperty property)
        {
            Type propertyType = property.GetType();
            if (propertyDrawersToPropertyTypes.ContainsKey(propertyType))
            {
                return propertyDrawersToPropertyTypes[propertyType].Copy(property, this.body, this.config);
            }
            else
            {
                Debug.LogError("There is no AnimatorToolPropertyDrawer for " + propertyType.Name + ".");
                return null;
            }
        }

        private void HandleUndoRedo()
        {
            this.isDraggingPreview = false;
            this.isDraggingOrientation = false;

            if (this.body != null)
            {
                this.UpdateEditPropertyIndex(this.editPropertyIndex);
                this.UpdateEditAnimationIndex(this.editAnimationIndex);
            }
        }

        private void UpdateEditPropertyIndex(int newPropertyIndex)
        {
            if (this.body.GetNumberProperties() == 0)
            {
                this.editPropertyIndex = -1;
                return;
            }
            this.editPropertyIndex = Mathf.Clamp(newPropertyIndex, 0, this.body.GetNumberProperties() - 1);
        }

        private void UpdateEditAnimationIndex(int newAnimationIndex)
        {
            if (this.body.GetAnimationCount() == 0)
            {
                this.editAnimationIndex = -1;
            }
            else
            {
                this.editAnimationIndex = Mathf.Clamp(newAnimationIndex, 0, this.body.GetAnimationCount() - 1);
            }

            this.UpdateEditFrameIndex(this.editFrameIndex);
            this.UpdateEditOrientationIndex(this.editOrientationIndex);
        }

        private void UpdateEditFrameIndex(int newFrameIndex)
        {
            if (this.editAnimationIndex != -1 && this.body.GetFrameCount(this.editAnimationIndex) > 0)
            {
                this.editFrameIndex = Mathf.Clamp(newFrameIndex, 0, this.body.GetFrameCount(this.editAnimationIndex) - 1);
            }
            else
            {
                this.editFrameIndex = -1;
            }
        }

        private void UpdateEditOrientationIndex(int newOrientationIndex)
        {
            if (this.editAnimationIndex != -1 && this.body.GetOrientationCount(this.editAnimationIndex) > 0)
            {
                this.editOrientationIndex = Mathf.Clamp(newOrientationIndex, 0, this.body.GetOrientationCount(this.editAnimationIndex) - 1);
            }
            else
            {
                this.editOrientationIndex = -1;
            }
        }

        private void DrawAnimationSelectionArea()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (this.body.GetAnimationCount() == 0)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.LabelField("No Animations", EditorStyles.toolbarTextField);
                        }
                    }
                    else
                    {
                        Rect popupRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);

                        string[] popupNames = new string[this.body.GetAnimationCount()];
                        for (int animationIndex = 0; animationIndex < this.body.GetAnimationCount(); animationIndex++)
                        {
                            popupNames[animationIndex] = "[" + animationIndex + "] " + this.body.GetAnimationName(animationIndex);
                        }
                        using (new EditorGUI.DisabledScope(Event.current.control))
                        {
                            this.UpdateEditAnimationIndex(EditorGUI.Popup(popupRect, this.editAnimationIndex, popupNames, EditorStyles.toolbarDropDown));
                        }

                        this.body.SetAnimationName(this.editAnimationIndex, EditorGUILayout.TextField(this.body.GetAnimationName(this.editAnimationIndex), EditorStyles.toolbarTextField));

                        if (Event.current.control && popupRect.Contains(Event.current.mousePosition))
                        {
                            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                            {
                                if (Event.current.button == 0)
                                {
                                    if (this.copyPropertyIndex == -1 &&
                                        this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                        this.copyFrameIndex == -1 &&
                                        this.copyOrientationIndex == -1 &&
                                        this.editAnimationIndex != -1)
                                    {
                                        this.body.CopyPasteAnimation(this.copyAnimationIndex, this.editAnimationIndex);
                                        Event.current.Use();
                                    }
                                }
                                else if (Event.current.button == 1)
                                {
                                    this.copyPropertyIndex = -1;
                                    this.copyAnimationIndex = this.editAnimationIndex;
                                    this.copyFrameIndex = -1;
                                    this.copyOrientationIndex = -1;
                                    Event.current.Use();
                                }
                            }
                            using (new UtilityGUI.ColorScope(this.config.LightColor))
                            {
                                UtilityGUI.DrawOutlineBox(popupRect, this.config.CopyPasteOutlineThickness);
                            }
                        }
                    }
                }

                int addRemoveMaxWidth = 100;
                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.MaxWidth(addRemoveMaxWidth)))
                {
                    this.body.AddAnimationAt(this.editAnimationIndex + 1);
                    this.UpdateEditAnimationIndex(this.editAnimationIndex);
                }
                using (new EditorGUI.DisabledScope(this.editAnimationIndex == -1))
                {
                    if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.MaxWidth(addRemoveMaxWidth)))
                    {
                        this.body.RemoveAnimationAt(this.editAnimationIndex);
                        this.UpdateEditAnimationIndex(this.editAnimationIndex);
                    }
                }

                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                {
                    if (GUILayout.Button("Set Asset Preview", EditorStyles.toolbarButton))
                    {
                        this.body.Preview(0, 0f, Vector2.zero);
                    }
                }
            }
        }

        private void DrawPreviewArea()
        {
            this.DrawPreviewBar();
            using (new EditorGUILayout.HorizontalScope())
            {
                this.DrawPropertyEditFields();
                this.DrawPreviewSquare();
                this.DrawOrientationBar();
            }
        }

        private void DrawPreviewBar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                this.previewNumberTiles = EditorGUILayout.IntSlider("Preview Number Tiles", this.previewNumberTiles, this.config.PreviewMinNumberTiles, this.config.PreviewMaxNumberTiles);
                if (GUILayout.Button("Center Preview", EditorStyles.toolbarButton))
                {
                    this.previewOffset = Vector2.zero;
                }
            }
        }

        private void DrawPropertyEditFields()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth((EditorGUIUtility.currentViewWidth - this.config.PreviewLength) / 2f), GUILayout.MaxHeight(this.config.PreviewLength)))
            {
                if (this.editPropertyIndex < this.body.GetNumberProperties())
                {
                    AnimatorProperty currentProperty = this.body.GetProperties()[this.editPropertyIndex];
                    AnimatorToolPropertyDrawer propertyDrawer = this.CreatePropertyDrawerForProperty(currentProperty);

                    this.editCategoryIndex = GUILayout.Toolbar(this.editCategoryIndex, new string[] { "General Fields", "Animation Fields", "Frame Fields", "Orientation Fields" }, EditorStyles.toolbarButton);
                    using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.editFieldsScrollPos, EditorStyles.helpBox))
                    {
                        this.editFieldsScrollPos = scrollViewScope.scrollPosition;
                        switch (this.editCategoryIndex)
                        {
                            case 0:
                                propertyDrawer.DrawGeneralEditFields();
                                break;
                            case 1:
                                if (this.editAnimationIndex != -1)
                                {
                                    propertyDrawer.DrawAnimationEditFields(this.editAnimationIndex);
                                }
                                break;
                            case 2:
                                if (this.editAnimationIndex != -1 && this.editFrameIndex != -1)
                                {
                                    propertyDrawer.DrawFrameEditFields(this.editAnimationIndex, this.editFrameIndex);
                                }
                                break;
                            case 3:
                                if (this.editAnimationIndex != -1 && this.editFrameIndex != -1 && this.editOrientationIndex != -1)
                                {
                                    propertyDrawer.DrawOrientationEditFields(this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void DrawPreviewSquare()
        {
            Rect previewRect = GUILayoutUtility.GetRect(this.config.PreviewLength, this.config.PreviewLength, GUILayout.Width(this.config.PreviewLength), GUILayout.Height(this.config.PreviewLength));
            using (new GUI.GroupScope(previewRect))
            {
                float tileWindowLength = (float)this.config.PreviewLength / this.previewNumberTiles;
                bool showLightPanel = true;
                for (int x = 0; x < this.previewNumberTiles; x++)
                {
                    for (int y = 0; y < this.previewNumberTiles; y++)
                    {
                        Rect previewPanelRect = new Rect(
                            new Vector2(tileWindowLength * x, tileWindowLength * y),
                            new Vector2(tileWindowLength, tileWindowLength)
                            );
                        if (showLightPanel)
                        {
                            GUI.DrawTexture(previewPanelRect, this.config.DarkPreviewPanelTexture);
                        }
                        else
                        {
                            GUI.DrawTexture(previewPanelRect, this.config.LightPreviewPanelTexture);
                        }
                        showLightPanel = !showLightPanel;
                    }
                    if (this.previewNumberTiles % 2 == 0)
                    {
                        showLightPanel = !showLightPanel;
                    }
                }

                using (new UtilityGUI.ColorScope(Color.red))
                {
                    UtilityGUI.DrawLine(previewRect.size / 2 + new Vector2(-this.config.PreviewLength / 2, this.previewOffset.y), previewRect.size / 2 + new Vector2(this.config.PreviewLength / 2, this.previewOffset.y));
                }
                using (new UtilityGUI.ColorScope(Color.blue))
                {
                    UtilityGUI.DrawLine(previewRect.size / 2 + new Vector2(this.previewOffset.x, -this.config.PreviewLength / 2), previewRect.size / 2 + new Vector2(this.previewOffset.x, this.config.PreviewLength / 2));
                }

                List<AnimatorProperty> properties = this.body.GetProperties();
                List<(Rect rect, Action onDrag)>[] dragRequestsPerProperty = new List<(Rect rect, Action onDrag)>[properties.Count];

                if (this.editAnimationIndex != -1 && this.editFrameIndex != -1 && this.editOrientationIndex != -1)
                {
                    void DrawPropertiesAtLevel(AnimatorProperty[] currentProperties)
                    {
                        List<(AnimatorProperty property, AnimatorToolPropertyDrawer drawer)> drawersAndProperties = new List<(AnimatorProperty property, AnimatorToolPropertyDrawer drawer)>();
                        foreach (AnimatorProperty currentProperty in currentProperties)
                        {
                            drawersAndProperties.Add((currentProperty, this.CreatePropertyDrawerForProperty(currentProperty)));
                        }
                        drawersAndProperties.Sort(
                            ((AnimatorProperty property, AnimatorToolPropertyDrawer drawer) l, (AnimatorProperty property, AnimatorToolPropertyDrawer drawer) r) =>
                            {
                                int lIndex = properties.IndexOf(l.property);
                                int rIndex = properties.IndexOf(r.property);
                                int lOrder = lIndex == this.editPropertyIndex ? 1 : 0;
                                int rOrder = rIndex == this.editPropertyIndex ? 1 : 0;
                                if (lOrder == rOrder)
                                {
                                    float[] lOrders = l.drawer.CalculateChildPreviewOrders(this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex);
                                    float[] rOrders = r.drawer.CalculateChildPreviewOrders(this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex);
                                    int count = Mathf.Min(lOrders.Length, rOrders.Length);
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (Mathf.Abs(lOrders[i] - rOrders[i]) <= Mathf.Epsilon) continue;
                                        float diff = lOrders[i] - rOrders[i];
                                        if (diff > 0) return Mathf.CeilToInt(diff);
                                        else return Mathf.FloorToInt(diff);
                                    }
                                }
                                return lOrder - rOrder;
                            }
                            );

                        foreach ((AnimatorProperty property, AnimatorToolPropertyDrawer drawer) drawerAndProperty in drawersAndProperties)
                        {
                            AnimatorProperty property = drawerAndProperty.property;
                            AnimatorToolPropertyDrawer drawer = drawerAndProperty.drawer;

                            Vector2 translation = property.GetLocalPosition(this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex) * new Vector2(1, -1) * tileWindowLength;
                            float rotation = -property.GetLocalRotation(this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex);

                            Matrix4x4 prevMatrix = GUI.matrix;
                            // Workaround to use GUIUtility.RotateAroundPivot. Using Matrix4x4.Rotate has this weird offsetting issue that only Unity knows how to handle it seems.
                            GUI.matrix = Matrix4x4.identity;
                            GUIUtility.RotateAroundPivot(rotation, previewRect.size / 2 + translation);
                            Matrix4x4 rotationMatrix = GUI.matrix;
                            Matrix4x4 translationMatrix = Matrix4x4.Translate(translation);
                            GUI.matrix = prevMatrix * rotationMatrix * translationMatrix;

                            if (property.GetBinded(this.editAnimationIndex))
                            {
                                int propertyIndex = properties.IndexOf(property);
                                drawer.DrawPreview(previewRect.size, tileWindowLength, this.editAnimationIndex, this.editFrameIndex, this.editOrientationIndex, propertyIndex == this.editPropertyIndex);
                            }

                            AnimatorProperty[] nextLevelProperties = new AnimatorProperty[property.GetNumberChildProperties()];
                            for (int childIndex = 0; childIndex < property.GetNumberChildProperties(); childIndex++)
                            {
                                nextLevelProperties[childIndex] = property.GetChildPropertyAt(childIndex);
                            }

                            DrawPropertiesAtLevel(nextLevelProperties);

                            GUI.matrix = prevMatrix;
                        }
                    }
                    Matrix4x4 prevMatrix = GUI.matrix;
                    GUI.matrix = Matrix4x4.Translate(this.previewOffset);
                    DrawPropertiesAtLevel(new AnimatorProperty[] { this.body.RootProperty });
                    GUI.matrix = prevMatrix;
                }

                if (Event.current.type == EventType.MouseDown)
                {
                    if (new Rect(Vector2.zero, previewRect.size).Contains(Event.current.mousePosition))
                    {
                        if (Event.current.button == 2)
                        {
                            if (!this.isDraggingPreview)
                            {
                                this.isDraggingPreview = true;
                                Event.current.Use();
                            }
                        }
                    }
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    if (Event.current.button == 2)
                    {
                        if (this.isDraggingPreview)
                        {
                            this.previewOffset += Event.current.delta;
                            Event.current.Use();
                        }
                    }
                }
                else if (Event.current.type == EventType.MouseUp && this.isDraggingPreview)
                {
                    if (Event.current.button == 2)
                    {
                        this.isDraggingPreview = false;
                        Event.current.Use();
                    }
                }
            }
        }

        private void DrawOrientationBar()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.MaxHeight(this.config.PreviewLength)))
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    this.body.RotateToDirection = EditorGUILayout.Toggle("Rotate To Direction", this.body.RotateToDirection);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    Rect dragAreaRect = GUILayoutUtility.GetRect(this.config.OrientationBarLength, this.config.OrientationBarLength, GUILayout.Width(this.config.OrientationBarLength), GUILayout.Height(this.config.OrientationBarLength));
                    using (new UtilityGUI.ColorScope(this.config.LightColor))
                    {
                        UtilityGUI.DrawSolidBox(dragAreaRect);
                    }

                    Rect innerBorderRect = new Rect(dragAreaRect);
                    innerBorderRect.xMin += this.config.BorderLength;
                    innerBorderRect.xMax -= this.config.BorderLength;
                    innerBorderRect.yMin += this.config.BorderLength;
                    innerBorderRect.yMax -= this.config.BorderLength;
                    using (new UtilityGUI.ColorScope(this.config.DarkColor))
                    {
                        UtilityGUI.DrawSolidBox(innerBorderRect);
                    }

                    if (this.editAnimationIndex != -1)
                    {
                        float divisionAngleRad = Mathf.PI * 2 / this.body.GetOrientationCount(this.editAnimationIndex);

                        Rect[] buttonRects = new Rect[this.body.GetOrientationCount(this.editAnimationIndex)];
                        for (int orientationIndex = 0; orientationIndex < this.body.GetOrientationCount(this.editAnimationIndex); orientationIndex++)
                        {
                            Vector2 size = new Vector2(this.config.OrientationDragButtonLength, this.config.OrientationDragButtonLength);
                            Vector2 drawOffset = this.body.GetOrientationDirection(this.editAnimationIndex, orientationIndex) * this.config.OrientationDragCircleRadius;
                            if (drawOffset.magnitude == 0)
                            {
                                drawOffset = new Vector2(Mathf.Cos(divisionAngleRad * orientationIndex), Mathf.Sin(divisionAngleRad * orientationIndex)) * this.config.OrientationZeroCircleRadius;
                            }
                            Vector2 position = dragAreaRect.center + drawOffset * new Vector2(1, -1) - size / 2;
                            Rect buttonRect = new Rect(position, size);
                            buttonRects[orientationIndex] = buttonRect;
                            if (orientationIndex != this.editOrientationIndex)
                            {
                                if (GUI.Button(buttonRect, "", UtilityStyles.EmptyStyle))
                                {
                                    this.UpdateEditOrientationIndex(orientationIndex);
                                }
                                using (new UtilityGUI.ColorScope(this.config.MediumColor))
                                {
                                    GUI.DrawTexture(buttonRect, this.config.OrientationButtonTexture);
                                }
                            }
                            else
                            {
                                using (new UtilityGUI.ColorScope(this.config.LightColor))
                                {
                                    GUI.DrawTexture(buttonRect, this.config.OrientationButtonTexture);
                                }
                            }
                        }

                        if (this.editOrientationIndex != -1 &&
                            !this.isDraggingOrientation &&
                            Event.current.type == EventType.MouseDown &&
                            Event.current.button == 0)
                        {
                            if (buttonRects[this.editOrientationIndex].Contains(Event.current.mousePosition))
                            {
                                this.isDraggingOrientation = true;
                                Event.current.Use();
                            }
                        }
                        else if (Event.current.type == EventType.MouseDrag && this.isDraggingOrientation)
                        {
                            Vector2 dragDirection = (Event.current.mousePosition - dragAreaRect.center).normalized;
                            if (Vector2.Distance(Event.current.mousePosition, dragAreaRect.center) < this.config.OrientationZeroDirectionThresholdDistance)
                            {
                                dragDirection = Vector2.zero;
                            }
                            if (Event.current.shift)
                            {
                                float shiftDivisionAngleRad = Mathf.PI * 2 / this.numShiftDivisions;
                                float angleRad = Mathf.Atan2(dragDirection.y, dragDirection.x);
                                float shiftAngleRad = Mathf.RoundToInt(angleRad / shiftDivisionAngleRad) * shiftDivisionAngleRad;
                                dragDirection = new Vector2(Mathf.Cos(shiftAngleRad), Mathf.Sin(shiftAngleRad));
                            }
                            Vector2 newOrientationDirection = new Vector2(dragDirection.x, -dragDirection.y);
                            if (newOrientationDirection != this.body.GetOrientationDirection(this.editAnimationIndex, this.editOrientationIndex))
                            {
                                Undo.RecordObject(this.body, "Dragged Orientation Direction");
                                this.body.SetOrientationDirection(this.editAnimationIndex, this.editOrientationIndex, newOrientationDirection);
                            }
                            Event.current.Use();
                        }
                        else if (Event.current.type == EventType.MouseUp && this.isDraggingOrientation)
                        {
                            this.isDraggingOrientation = false;
                            Event.current.Use();
                        }
                    }

                    GUILayout.FlexibleSpace();
                }

                using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.orientationBarScrollPos, EditorStyles.helpBox))
                {
                    this.orientationBarScrollPos = scrollViewScope.scrollPosition;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledScope(this.editAnimationIndex == -1))
                        {
                            if (this.editAnimationIndex != -1)
                            {
                                this.body.SetOrientationCount(this.editAnimationIndex, EditorGUILayout.IntField("Orientation Count", this.body.GetOrientationCount(this.editAnimationIndex)));
                            }

                            if (GUILayout.Button("+"))
                            {
                                this.body.AddOrientationAt(this.editAnimationIndex, this.editOrientationIndex + 1);
                                this.UpdateEditOrientationIndex(this.editOrientationIndex);
                            }

                            using (new EditorGUI.DisabledScope(this.editOrientationIndex == -1))
                            {
                                if (GUILayout.Button("-"))
                                {
                                    this.body.RemoveOrientationAt(this.editAnimationIndex, this.editOrientationIndex);
                                    this.UpdateEditOrientationIndex(this.editOrientationIndex);
                                }
                            }
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledScope(this.editAnimationIndex == -1))
                        {
                            if (GUILayout.Button("Distribute Evenly"))
                            {
                                float divisionAngleRad = Mathf.PI * 2 / this.body.GetOrientationCount(this.editAnimationIndex);
                                for (int orientationIndex = 0; orientationIndex < this.body.GetOrientationCount(this.editAnimationIndex); orientationIndex++)
                                {
                                    float angleRad = -Mathf.PI / 2 + orientationIndex * divisionAngleRad;
                                    this.body.SetOrientationDirection(this.editAnimationIndex, orientationIndex, new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)));
                                }
                            }

                            if (GUILayout.Button("Zero All"))
                            {
                                for (int orientationIndex = 0; orientationIndex < this.body.GetOrientationCount(this.editAnimationIndex); orientationIndex++)
                                {
                                    this.body.SetOrientationDirection(this.editAnimationIndex, orientationIndex, Vector2.zero);
                                }
                            }
                        }
                    }

                    this.numShiftDivisions = Mathf.Max(1, EditorGUILayout.IntField("Shift Divisions", this.numShiftDivisions));

                    if (this.editAnimationIndex != -1)
                    {
                        for (int orientationIndex = 0; orientationIndex < this.body.GetOrientationCount(this.editAnimationIndex); orientationIndex++)
                        {
                            using (new EditorGUI.DisabledScope(this.editOrientationIndex != orientationIndex))
                            {
                                this.body.SetOrientationDirection(this.editAnimationIndex, orientationIndex, EditorGUILayout.Vector2Field("", this.body.GetOrientationDirection(this.editAnimationIndex, orientationIndex)));
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
        }

        private void DrawTimelineArea()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                this.DrawFrameBar();
                this.DrawCells();
            }
        }

        private void DrawFrameBar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (this.editAnimationIndex != -1)
                {
                    this.body.SetFrameCount(this.editAnimationIndex, EditorGUILayout.IntField("Frame Count", this.body.GetFrameCount(this.editAnimationIndex)));
                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - this.config.OrientationBarLength)))
                    {
                        using (new EditorGUI.DisabledScope(this.framePageIndex == 0))
                        {
                            if (GUILayout.Button("Previous Page", EditorStyles.toolbarButton))
                            {
                                this.framePageIndex--;
                            }
                        }
                        using (new EditorGUI.DisabledScope((this.framePageIndex + 1) * this.config.NumberFramesPerPage - this.body.GetFrameCount(this.editAnimationIndex) >= 0))
                        {
                            if (GUILayout.Button("Next Page", EditorStyles.toolbarButton))
                            {
                                this.framePageIndex++;
                            }
                        }
                        using (new EditorGUI.DisabledScope(this.editFrameIndex == -1))
                        {
                            if (GUILayout.Button("Add Frame Left", EditorStyles.toolbarButton))
                            {
                                this.body.AddFrameAt(this.editAnimationIndex, this.editFrameIndex);
                                this.UpdateEditFrameIndex(this.editFrameIndex + 1);
                            }
                        }
                        if (GUILayout.Button("Add Frame Right", EditorStyles.toolbarButton))
                        {
                            this.body.AddFrameAt(this.editAnimationIndex, this.editFrameIndex + 1);
                            this.UpdateEditFrameIndex(this.editFrameIndex);
                        }
                        using (new EditorGUI.DisabledScope(this.editFrameIndex == -1))
                        {
                            if (GUILayout.Button("Delete Frame", EditorStyles.toolbarButton))
                            {
                                this.body.RemoveFrameAt(this.editAnimationIndex, this.editFrameIndex);
                                this.UpdateEditFrameIndex(this.editFrameIndex);
                            }
                        }
                    }

                    this.body.SetFrameRate(this.editAnimationIndex, Mathf.Max(0, EditorGUILayout.FloatField("Frame Rate", this.body.GetFrameRate(this.editAnimationIndex))));

                    this.body.SetLoopBehaviour(this.editAnimationIndex, (AnimatorBody.LoopBehaviour)EditorGUILayout.EnumPopup(this.body.GetLoopBehaviour(this.editAnimationIndex)));
                    using (new EditorGUI.DisabledScope(this.body.GetLoopBehaviour(this.editAnimationIndex) != AnimatorBody.LoopBehaviour.LoopForDuration))
                    {
                        this.body.SetLoopDuration(this.editAnimationIndex, EditorGUILayout.FloatField(this.body.GetLoopDuration(this.editAnimationIndex)));
                    }
                }
                else
                {
                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawCells()
        {
            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.cellsScrollPos))
            {
                this.cellsScrollPos = scrollViewScope.scrollPosition;

                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(this.config.BorderLength)))
                {
                    Rect dividerRect = GUILayoutUtility.GetRect(0, this.config.BorderLength);
                    using (new UtilityGUI.ColorScope(this.config.DarkColor))
                    {
                        UtilityGUI.DrawSolidBox(dividerRect);
                    }
                }

                int pageStartIndex = 0;
                int pageEndIndex = 0;
                if (this.editAnimationIndex != -1)
                {
                    pageStartIndex = this.framePageIndex * this.config.NumberFramesPerPage;
                    pageEndIndex = Mathf.Min(this.body.GetFrameCount(this.editAnimationIndex), pageStartIndex + this.config.NumberFramesPerPage);
                }

                int totalIndentLevel = this.body.GetPropertyDepth();
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(totalIndentLevel * this.config.IndentWidth + this.config.PropertyNameWidth + this.config.PropertyBindWidth + (pageEndIndex - pageStartIndex + 1) * this.config.CellLength)))
                {
                    void DrawCellRow(int indentLevel, Action<Rect> onDrawIndent, Action<Rect> onDrawControls, Action<Rect> onDrawPropertyCell, Action<Rect, int> onDrawFrameCell, Color emptyCellsColor)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            float emptyIndentLength = (indentLevel - Mathf.Min(indentLevel, 1)) * this.config.IndentWidth;
                            GUILayoutUtility.GetRect(emptyIndentLength, this.config.CellLength, GUILayout.Width(emptyIndentLength), GUILayout.Height(this.config.CellLength));

                            if (indentLevel > 0)
                            {
                                Rect indentRect = GUILayoutUtility.GetRect(this.config.IndentWidth, this.config.CellLength, GUILayout.Width(this.config.IndentWidth), GUILayout.Height(this.config.CellLength));
                                onDrawIndent.Invoke(indentRect);
                            }

                            float filledIndentLength = (totalIndentLevel - indentLevel) * this.config.IndentWidth;
                            Rect filledIndentRect = GUILayoutUtility.GetRect(filledIndentLength, this.config.CellLength, GUILayout.Width(filledIndentLength), GUILayout.Height(this.config.CellLength));
                            using (new UtilityGUI.ColorScope(this.config.DarkColor))
                            {
                                UtilityGUI.DrawSolidBox(filledIndentRect);
                            }

                            float controlsLength = this.config.PropertyNameWidth + this.config.PropertyBindWidth;
                            Rect controlsRect = GUILayoutUtility.GetRect(controlsLength, this.config.CellLength, GUILayout.Width(controlsLength), GUILayout.Height(this.config.CellLength));
                            using (new UtilityGUI.ColorScope(this.config.DarkColor))
                            {
                                UtilityGUI.DrawSolidBox(controlsRect);
                            }
                            onDrawControls.Invoke(controlsRect);

                            Rect propertyCellRect = GUILayoutUtility.GetRect(this.config.CellLength, this.config.CellLength, GUILayout.Width(this.config.CellLength), GUILayout.Height(this.config.CellLength));
                            onDrawPropertyCell?.Invoke(propertyCellRect);

                            float emptyRectLength = EditorGUIUtility.currentViewWidth - this.config.OrientationBarLength;
                            for (int frameIndex = pageStartIndex; frameIndex < pageEndIndex; frameIndex++)
                            {
                                using (new EditorGUILayout.VerticalScope(GUILayout.Width(this.config.CellLength)))
                                {
                                    Rect frameCellRect = GUILayoutUtility.GetRect(this.config.CellLength, this.config.CellLength, GUILayout.Width(this.config.CellLength), GUILayout.Height(this.config.CellLength));
                                    onDrawFrameCell?.Invoke(frameCellRect, frameIndex);
                                }
                                emptyRectLength -= this.config.CellLength;
                            }


                            Rect emptyRect = GUILayoutUtility.GetRect(emptyRectLength, this.config.CellLength);
                            using (new UtilityGUI.ColorScope(emptyCellsColor))
                            {
                                GUI.DrawTexture(emptyRect, this.config.EmptyCellTexture);
                            }
                        }
                    }

                    List<Rect> copyPasteOutlineRects = new List<Rect>();
                    List<AnimatorProperty> properties = this.body.GetProperties();

                    int numExpandedProperties = 0;
                    foreach (AnimatorProperty property in properties)
                    {
                        if (InternalEditorUtility.GetIsInspectorExpanded(property))
                        {
                            numExpandedProperties++;
                        }
                    }

                    DrawCellRow(
                        0,
                        (Rect indentRect) => { },
                        (Rect controlsRect) => { },
                        (Rect propertyCellRect) =>
                        {
                            using (new UtilityGUI.ColorScope(this.config.MediumColor))
                            {
                                GUI.DrawTexture(propertyCellRect, this.config.CornerCellTexture);
                            }
                            if (Event.current.control && propertyCellRect.Contains(Event.current.mousePosition))
                            {
                                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                {
                                    if (Event.current.button == 0)
                                    {
                                        if (this.copyPropertyIndex == -1 &&
                                            this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                            this.copyFrameIndex == -1 &&
                                            this.copyOrientationIndex == -1 &&
                                            this.editAnimationIndex != -1)
                                        {
                                            this.body.CopyPasteAllFramesAndTheirOrientationsAcrossAllProperties(this.copyAnimationIndex, this.editAnimationIndex);
                                            Event.current.Use();
                                        }
                                    }
                                    else if (Event.current.button == 1)
                                    {
                                        this.copyPropertyIndex = -1;
                                        this.copyAnimationIndex = this.editAnimationIndex;
                                        this.copyFrameIndex = -1;
                                        this.copyOrientationIndex = -1;
                                        Event.current.Use();
                                    }
                                }
                                Rect copyPasteOutlineRect = new Rect(propertyCellRect);
                                copyPasteOutlineRect.height += (this.body.GetNumberProperties() + numExpandedProperties * (this.editAnimationIndex == -1 ? 0 : this.body.GetOrientationCount(this.editAnimationIndex))) * this.config.CellLength;
                                copyPasteOutlineRect.width += this.config.CellLength * (this.editAnimationIndex == -1 ? 0 : this.body.GetFrameCount(this.editAnimationIndex));
                                copyPasteOutlineRects.Add(copyPasteOutlineRect);
                            }
                        },
                        (Rect frameCellRect, int frameIndex) =>
                        {
                            if (!Event.current.control && GUI.Button(frameCellRect, "", UtilityStyles.EmptyStyle))
                            {
                                this.UpdateEditFrameIndex(frameIndex);
                            }
                            using (new UtilityGUI.ColorScope(this.editFrameIndex == frameIndex ? this.config.LightColor : this.config.MediumColor))
                            {
                                GUI.DrawTexture(frameCellRect, this.config.ColumnMarkerCellTexture);
                            }
                            if (Event.current.control && frameCellRect.Contains(Event.current.mousePosition))
                            {
                                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                {
                                    if (Event.current.button == 0)
                                    {
                                        if (this.copyPropertyIndex == -1 &&
                                            this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                            this.copyFrameIndex >= 0 && this.copyFrameIndex < this.body.GetFrameCount(this.copyAnimationIndex) &&
                                            this.copyOrientationIndex == -1 &&
                                            this.editAnimationIndex != -1)
                                        {
                                            this.body.CopyPasteFrameAndItsOrientationsAcrossAllProperties(this.copyAnimationIndex, this.editAnimationIndex, this.copyFrameIndex, frameIndex);
                                            Event.current.Use();
                                        }
                                    }
                                    else if (Event.current.button == 1)
                                    {
                                        this.copyPropertyIndex = -1;
                                        this.copyAnimationIndex = this.editAnimationIndex;
                                        this.copyFrameIndex = frameIndex;
                                        this.copyOrientationIndex = -1;
                                        Event.current.Use();
                                    }
                                }
                                Rect copyPasteOutlineRect = new Rect(frameCellRect);
                                copyPasteOutlineRect.height += (this.body.GetNumberProperties() + numExpandedProperties * (this.editAnimationIndex == -1 ? 0 : this.body.GetOrientationCount(this.editAnimationIndex))) * this.config.CellLength;
                                copyPasteOutlineRects.Add(copyPasteOutlineRect);
                            }
                            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                            labelStyle.alignment = TextAnchor.UpperLeft;
                            labelStyle.padding.left += this.config.FrameNumberPadding;
                            labelStyle.padding.top += this.config.FrameNumberPadding;
                            GUI.Label(frameCellRect, (frameIndex + 1).ToString(), labelStyle);
                        },
                        this.config.MediumColor
                        );

                    for (int propertyIndex = 0; propertyIndex < this.body.GetNumberProperties(); propertyIndex++)
                    {
                        AnimatorProperty property = properties[propertyIndex];

                        AnimatorToolPropertyDrawer propertyDrawer = this.CreatePropertyDrawerForProperty(property);

                        Color GetSelectColor(bool useLighterShade)
                        {
                            Color cellColor = useLighterShade ? this.config.DarkColor : this.config.IndentColor;
                            if (this.editAnimationIndex != -1)
                            {
                                if (property == this.body.RootProperty || property.GetBinded(this.editAnimationIndex))
                                {
                                    cellColor = useLighterShade ? this.config.LightColor : this.config.MediumColor;
                                }
                                else
                                {
                                    cellColor = useLighterShade ? this.config.DarkColor : this.config.IndentColor;
                                }
                            }
                            return cellColor;
                        }

                        DrawCellRow(
                            this.body.GetPropertyDepthOf(property),
                            (Rect indentRect) =>
                            {
                                using (new UtilityGUI.ColorScope(this.config.DarkColor))
                                {
                                    GUI.DrawTexture(indentRect, this.config.IndentArrowTexture);
                                }
                            },
                            (Rect controlsRect) =>
                            {
                                // Draw Label
                                Rect labelRect = new Rect(controlsRect);
                                labelRect.width = this.config.PropertyNameWidth;
                                labelRect.height = this.config.CellLength / 2;
                                InternalEditorUtility.SetIsInspectorExpanded(property, GUI.Toggle(labelRect, InternalEditorUtility.GetIsInspectorExpanded(property), "", UtilityStyles.EmptyStyle));
                                using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(propertyDrawer.AccentColor, InternalEditorUtility.GetIsInspectorExpanded(property) ? 1 : 2)))
                                {
                                    UtilityGUI.DrawSolidBox(labelRect);
                                }
                                GUI.Label(labelRect, propertyDrawer.LabelName, UtilityStyles.WordWrapAndCenter(EditorStyles.boldLabel));

                                // Draw Name
                                Rect nameRect = new Rect(labelRect);
                                nameRect.position += Vector2.up * this.config.CellLength / 2;
                                property.PropertyName = GUI.TextField(nameRect, property.PropertyName, UtilityStyles.WordWrapAndCenter(EditorStyles.textField));

                                // Draw Bind
                                Rect bindRect = new Rect(labelRect);
                                bindRect.position += Vector2.right * this.config.PropertyNameWidth;
                                bindRect.height = this.config.CellLength;
                                bindRect.width = this.config.PropertyBindWidth;
                                if (property != this.body.RootProperty)
                                {
                                    if (this.editAnimationIndex != -1 && GUI.Button(bindRect, "", UtilityStyles.EmptyStyle))
                                    {
                                        property.SetBinded(this.editAnimationIndex, !property.GetBinded(this.editAnimationIndex));
                                    }
                                    using (new UtilityGUI.ColorScope(GetSelectColor(false)))
                                    {
                                        GUI.DrawTexture(bindRect, this.config.PropertyBindTexture);
                                    }
                                }
                                else
                                {
                                    using (new UtilityGUI.ColorScope(GetSelectColor(false)))
                                    {
                                        UtilityGUI.DrawSolidBox(bindRect);
                                    }
                                }
                            },
                            (Rect propertyCellRect) =>
                            {
                                if (!Event.current.control && GUI.Button(propertyCellRect, "", UtilityStyles.EmptyStyle))
                                {
                                    this.UpdateEditPropertyIndex(propertyIndex);
                                }
                                using (new UtilityGUI.ColorScope(GetSelectColor(this.editPropertyIndex == propertyIndex)))
                                {
                                    GUI.DrawTexture(propertyCellRect, this.config.FrameRowMarkerCellTexture);
                                }
                                if (Event.current.control && propertyCellRect.Contains(Event.current.mousePosition))
                                {
                                    if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                    {
                                        if (Event.current.button == 0)
                                        {
                                            if (this.copyPropertyIndex >= 0 && this.copyPropertyIndex < this.body.GetNumberProperties() &&
                                                this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                                this.copyFrameIndex == -1 &&
                                                this.copyOrientationIndex == -1 &&
                                                this.editAnimationIndex != -1)
                                            {
                                                this.body.CopyPasteAllFramesAndTheirOrientations(this.copyPropertyIndex, propertyIndex, this.copyAnimationIndex, this.editAnimationIndex);
                                                Event.current.Use();
                                            }
                                        }
                                        else if (Event.current.button == 1)
                                        {
                                            this.copyPropertyIndex = propertyIndex;
                                            this.copyAnimationIndex = this.editAnimationIndex;
                                            this.copyFrameIndex = -1;
                                            this.copyOrientationIndex = -1;
                                            Event.current.Use();
                                        }
                                    }
                                    Rect copyPasteOutlineRect = new Rect(propertyCellRect);
                                    copyPasteOutlineRect.width += this.config.CellLength * (this.editAnimationIndex == -1 ? 0 : this.body.GetFrameCount(this.editAnimationIndex));
                                    copyPasteOutlineRect.height += this.config.CellLength * (this.editAnimationIndex == -1 || !InternalEditorUtility.GetIsInspectorExpanded(property) ? 0 : this.body.GetOrientationCount(this.editAnimationIndex));
                                    copyPasteOutlineRects.Add(copyPasteOutlineRect);
                                }
                            },
                            (Rect frameCellRect, int frameIndex) =>
                            {
                                if (!Event.current.control && GUI.Button(frameCellRect, "", UtilityStyles.EmptyStyle))
                                {
                                    this.UpdateEditFrameIndex(frameIndex);
                                    this.UpdateEditPropertyIndex(propertyIndex);
                                }
                                using (new UtilityGUI.ColorScope(GetSelectColor(this.editPropertyIndex == propertyIndex && this.editFrameIndex == frameIndex)))
                                {
                                    GUI.DrawTexture(frameCellRect, this.config.MainCellTexture);
                                }
                                if (Event.current.control && frameCellRect.Contains(Event.current.mousePosition))
                                {
                                    if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                    {
                                        if (Event.current.button == 0)
                                        {
                                            if (this.copyPropertyIndex >= 0 && this.copyPropertyIndex < this.body.GetNumberProperties() &&
                                                this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                                this.copyFrameIndex >= 0 && this.copyFrameIndex < this.body.GetFrameCount(this.copyAnimationIndex) &&
                                                this.copyOrientationIndex == -1 &&
                                                this.editAnimationIndex != -1)
                                            {
                                                this.body.CopyPasteFrameAndItsOrientations(this.copyPropertyIndex, propertyIndex, this.copyAnimationIndex, this.editAnimationIndex, this.copyFrameIndex, frameIndex);
                                                Event.current.Use();
                                            }
                                        }
                                        else if (Event.current.button == 1)
                                        {
                                            this.copyPropertyIndex = propertyIndex;
                                            this.copyAnimationIndex = this.editAnimationIndex;
                                            this.copyFrameIndex = frameIndex;
                                            this.copyOrientationIndex = -1;
                                            Event.current.Use();
                                        }
                                    }
                                    Rect copyPasteOutlineRect = new Rect(frameCellRect);
                                    copyPasteOutlineRect.height += this.config.CellLength * (this.editAnimationIndex == -1 || !InternalEditorUtility.GetIsInspectorExpanded(property) ? 0 : this.body.GetOrientationCount(this.editAnimationIndex));
                                    copyPasteOutlineRects.Add(copyPasteOutlineRect);
                                }

                                if (this.editAnimationIndex != -1 && property.GetBinded(this.editAnimationIndex))
                                {
                                    Rect frameCellPreviewRect = new Rect(frameCellRect);
                                    frameCellPreviewRect.xMin += this.config.CellPreviewPadding;
                                    frameCellPreviewRect.xMax -= this.config.CellPreviewPadding;
                                    frameCellPreviewRect.yMin += this.config.CellPreviewPadding;
                                    frameCellPreviewRect.yMax -= this.config.CellPreviewPadding;
                                    using (new GUI.GroupScope(frameCellPreviewRect))
                                    {
                                        propertyDrawer.DrawFrameCellPreview(frameCellPreviewRect.size, this.editAnimationIndex, frameIndex);
                                    }
                                }
                            },
                            GetSelectColor(false)
                            );

                        if (this.editAnimationIndex != -1 && InternalEditorUtility.GetIsInspectorExpanded(property))
                        {
                            for (int orientationIndex = 0; orientationIndex < this.body.GetOrientationCount(this.editAnimationIndex); orientationIndex++)
                            {
                                DrawCellRow(
                                    this.body.GetPropertyDepthOf(property),
                                    (Rect indentRect) =>
                                    {
                                        indentRect.xMin += this.config.IndentWidth / 2;
                                        using (new UtilityGUI.ColorScope(this.config.DarkColor))
                                        {
                                            UtilityGUI.DrawSolidBox(indentRect);
                                        }
                                    },
                                    (Rect controlsRect) => 
                                    {
                                        Rect bindRect = new Rect(controlsRect);
                                        bindRect.x += this.config.PropertyNameWidth;
                                        bindRect.width = this.config.PropertyBindWidth;
                                        using (new UtilityGUI.ColorScope(GetSelectColor(false)))
                                        {
                                            UtilityGUI.DrawSolidBox(bindRect);
                                        }
                                    },
                                    (Rect propertyCellRect) =>
                                    {
                                        if (!Event.current.control && GUI.Button(propertyCellRect, "", UtilityStyles.EmptyStyle))
                                        {
                                            this.UpdateEditPropertyIndex(propertyIndex);
                                        }
                                        using (new UtilityGUI.ColorScope(GetSelectColor(this.editPropertyIndex == propertyIndex)))
                                        {
                                            GUI.DrawTexture(propertyCellRect, this.config.OrientationRowMarkerCellTexture);
                                        }
                                        if (Event.current.control && propertyCellRect.Contains(Event.current.mousePosition))
                                        {
                                            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                            {
                                                if (Event.current.button == 0)
                                                {
                                                    if (this.copyPropertyIndex >= 0 && this.copyPropertyIndex < this.body.GetNumberProperties() &&
                                                        this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                                        this.copyFrameIndex == -1 &&
                                                        this.copyOrientationIndex >= 0 && this.copyOrientationIndex < this.body.GetOrientationCount(this.copyAnimationIndex) &&
                                                        this.editAnimationIndex != -1)
                                                    {
                                                        this.body.CopyPasteOrientationAcrossAllFrames(this.copyPropertyIndex, propertyIndex, this.copyAnimationIndex, this.editAnimationIndex, this.copyOrientationIndex, orientationIndex);
                                                        Event.current.Use();
                                                    }
                                                }
                                                else if (Event.current.button == 1)
                                                {
                                                    this.copyPropertyIndex = propertyIndex;
                                                    this.copyAnimationIndex = this.editAnimationIndex;
                                                    this.copyFrameIndex = -1;
                                                    this.copyOrientationIndex = orientationIndex;
                                                    Event.current.Use();
                                                }
                                            }
                                            Rect copyPasteRowRect = new Rect(propertyCellRect);
                                            copyPasteRowRect.width += this.config.CellLength * (this.editAnimationIndex == -1 ? 0 : this.body.GetFrameCount(this.editAnimationIndex));
                                            copyPasteOutlineRects.Add(copyPasteRowRect);
                                        }
                                    },
                                    (Rect frameCellRect, int frameIndex) =>
                                    {
                                        if (!Event.current.control && GUI.Button(frameCellRect, "", UtilityStyles.EmptyStyle))
                                        {
                                            this.UpdateEditFrameIndex(frameIndex);
                                            this.UpdateEditPropertyIndex(propertyIndex);
                                            this.UpdateEditOrientationIndex(orientationIndex);
                                        }
                                        using (new UtilityGUI.ColorScope(GetSelectColor(this.editPropertyIndex == propertyIndex && this.editFrameIndex == frameIndex && this.editOrientationIndex == orientationIndex)))
                                        {
                                            GUI.DrawTexture(frameCellRect, this.config.MainCellTexture);
                                        }
                                        if (Event.current.control && frameCellRect.Contains(Event.current.mousePosition))
                                        {
                                            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                                            {
                                                if (Event.current.button == 0)
                                                {
                                                    if (this.copyPropertyIndex >= 0 && this.copyPropertyIndex < this.body.GetNumberProperties() &&
                                                        this.copyAnimationIndex >= 0 && this.copyAnimationIndex < this.body.GetAnimationCount() &&
                                                        this.copyFrameIndex >= 0 && this.copyFrameIndex < this.body.GetFrameCount(this.copyAnimationIndex) &&
                                                        this.copyOrientationIndex >= 0 && this.copyOrientationIndex < this.body.GetOrientationCount(this.copyAnimationIndex) &&
                                                        this.editAnimationIndex != -1)
                                                    {
                                                        this.body.CopyPasteOrientation(this.copyPropertyIndex, propertyIndex, this.copyAnimationIndex, this.editAnimationIndex, this.copyFrameIndex, frameIndex, this.copyOrientationIndex, orientationIndex);
                                                        Event.current.Use();
                                                    }
                                                }
                                                else if (Event.current.button == 1)
                                                {
                                                    this.copyPropertyIndex = propertyIndex;
                                                    this.copyAnimationIndex = this.editAnimationIndex;
                                                    this.copyFrameIndex = frameIndex;
                                                    this.copyOrientationIndex = orientationIndex;
                                                    Event.current.Use();
                                                }
                                            }
                                            copyPasteOutlineRects.Add(frameCellRect);
                                        }

                                        if (this.editAnimationIndex != -1 && property.GetBinded(this.editAnimationIndex))
                                        {
                                            Rect orientationCellPreviewRect = new Rect(frameCellRect);
                                            orientationCellPreviewRect.xMin += this.config.CellPreviewPadding;
                                            orientationCellPreviewRect.xMax -= this.config.CellPreviewPadding;
                                            orientationCellPreviewRect.yMin += this.config.CellPreviewPadding;
                                            orientationCellPreviewRect.yMax -= this.config.CellPreviewPadding;
                                            using (new GUI.GroupScope(orientationCellPreviewRect))
                                            {
                                                propertyDrawer.DrawOrientationCellPreview(orientationCellPreviewRect.size, this.editAnimationIndex, frameIndex, orientationIndex);
                                            }
                                        }
                                    },
                                    GetSelectColor(false)
                                    );
                            }
                        }
                    }

                    foreach (Rect copyPasteOutlineRect in copyPasteOutlineRects)
                    {
                        using (new UtilityGUI.ColorScope(this.config.LightColor))
                        {
                            UtilityGUI.DrawOutlineBox(copyPasteOutlineRect, this.config.CopyPasteOutlineThickness);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(this.config.BorderLength)))
                {
                    Rect dividerRect = GUILayoutUtility.GetRect(0, this.config.BorderLength);
                    using (new UtilityGUI.ColorScope(this.config.DarkColor))
                    {
                        UtilityGUI.DrawSolidBox(dividerRect);
                    }
                }
            }
        }
    }
}
#endif
