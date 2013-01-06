using System.Collections.Generic;
using System.Xml.Serialization;

namespace FlatRedBall_Spriter
{
    /// <remarks/>
    [XmlTypeAttribute(TypeName = "key", AnonymousType = true)]
    public partial class Key
    {
        /// <remarks/>
        [XmlElement("bone_ref", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<KeyBoneRef> BoneRef { get; set; }

        /// <remarks/>
        [XmlElement("object_ref", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<KeyObjectRef> ObjectRef { get; set; }

        /// <remarks/>
        [XmlElement("object", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public KeyObject Object { get; set; }

        /// <remarks/>
        [XmlElement("bone", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<KeyBone> Bone { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "time")]
        public int Time { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "spin")]
        public int Spin { get; set; }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class KeyBoneRef
    {
        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "timeline")]
        public int Timeline { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "key")]
        public int Key { get; set; }

        [XmlIgnore]
        public int? Parent { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "parent")]
        // ReSharper disable InconsistentNaming
        public int _donotuseParent
        {
            get { return -1; }
            set { this.Parent = value; }
        }

// ReSharper restore InconsistentNaming
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class KeyObjectRef
    {

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "parent")]
// ReSharper disable InconsistentNaming
        public int _donotuseParent
// ReSharper restore InconsistentNaming
        {
            get { return -1; }
            set { this.Parent = value; }
        }

        [XmlIgnore]
        public int? Parent { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "timeline")]
        public int Timeline { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "key")]
        public int Key { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "z_index")]
        public int ZIndex { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class KeyObject
    {
        private float _scalex = 1.0f;
        private float _scaley = 1.0f;
        

        /// <remarks/>
        [XmlAttribute(AttributeName = "folder")]
        public int Folder { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "file")]
        public int File { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "x")]
        public float X { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "y")]
        public float Y { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "angle")]
        public float Angle { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "scale_x")]
        public float ScaleX
        {
            get { return _scalex; }
            set { _scalex = value; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "scale_y")]
        public float ScaleY
        {
            get { return _scaley; }
            set { _scaley = value; }
        }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class KeyBone
    {
        private float _scaleX = 1.0f;
        private float _scaleY = 1.0f;

        /// <remarks/>
        [XmlAttribute(AttributeName = "x")]
        public float X { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "y")]
        public float Y { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "angle")]
        public float Angle { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "scale_x")]
        public float ScaleX
        {
            get { return _scaleX; }
            set { _scaleX = value; }
        }

        /// <remarks/>
        [XmlAttribute(AttributeName = "scale_y")]
        public float ScaleY
        {
            get { return _scaleY; }
            set { _scaleY = value; }
        }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "spriter_data", Namespace = "", IsNullable = false)]
    public partial class SpriterObjectSave
    {
        /// <remarks/>
        [XmlElement("folder", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<SpriterDataFolder> Folder { get; set; }

        /// <remarks/>
        [XmlElement("entity", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<SpriterDataEntity> Entity { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "scml_version")]
        public string ScmlVersion { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "generator")]
        public string Generator { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "generator_version")]
        public string GeneratorVersion { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "pixel_art_mode")]
        public bool PixelArtMode { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SpriterDataFolder
    {
        /// <remarks/>
        [XmlElement("file", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<SpriterDataFolderFile> File { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SpriterDataFolderFile
    {
        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "pivot_x")]
        public float PivotX { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "pivot_y")]
        public float PivotY { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "atlas_x")]
        public int AtlasX { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "atlas_y")]
        public int AtlasY { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "offset_x")]
        public int OffsetX { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "offset_y")]
        public int OffsetY { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "original_width")]
        public int OriginalWidth { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "original_height")]
        public int OriginalHeight { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SpriterDataEntity
    {
        /// <remarks/>
        [XmlElement("animation", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<SpriterDataEntityAnimation> Animation { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    public partial class SpriterDataEntityAnimationMainline
    {
        [XmlElement("key", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<Key> Keys { get; set; } 
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SpriterDataEntityAnimation
    {
        [XmlElement("mainline", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public SpriterDataEntityAnimationMainline Mainline { get; set; }

        /// <remarks/>
        [XmlElement("timeline", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<SpriterDataEntityAnimationTimeline> Timeline { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "length")]
        public int Length { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "looping")]
        public bool Looping { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class SpriterDataEntityAnimationTimeline
    {
        /// <remarks/>
        [XmlElement(ElementName = "key", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<Key> Key { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <remarks/>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class NewDataSet
    {
        /// <remarks/>
        [XmlElementAttribute("spriter_data")]
        public SpriterObjectSave Item { get; set; }

    }
}
