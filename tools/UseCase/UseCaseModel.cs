using System;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;

namespace UseCase
{
    public interface IItem
    {     
        string ToString();
    }

    public class Item : IItem
    {
        public Item( string text, XmlNode node )
        {
            this.text = text;
            this.node = node;
        }

        public Item( string text ) : this( text, null )
        {}

        public override string ToString()
        {
            return this.text;
        }

        public string Text
        {
            get{ return this.text; }
            set{ this.text = value; }
        }

        public XmlNode Node
        {
            get{ return this.node; }
            set{ this.node = value; }
        }

        private string text;
        private XmlNode node;
    }

	/// <summary>
	/// Summary description for UseCaseModel.
	/// </summary>
    public class UseCaseModel
    {
        public event EventHandler ActorsChanged;
        public event EventHandler PreConditionsChanged;
        public event EventHandler PostConditionsChanged;
        public event EventHandler ElementsChanged;
        public event EventHandler AtomsChanged;

        public event EventHandler Changed;

        public UseCaseModel()
        {
            this.doc = new XmlDocument();  
            this.PopulateDocument();

            this.SetupEventHandlers();
        }

        protected UseCaseModel( XmlDocument doc )
        {
            this.doc = doc;
            this.SetupEventHandlers();
        }

        public static UseCaseModel FromFile( string filename )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( filename );
            
            return new UseCaseModel( doc );
        }

        public string AsXml
        {
            get
            { 
                StringBuilder builder = new StringBuilder();
                this.FormatXml( this.doc.DocumentElement, builder, 0 );
                return builder.ToString();
            }
        }


        public IItem[] Actors
        {
            get { return this.GetItems( "/UseCase/Preface/Actors/ActorID" );  }
        }

        public IItem[] PreConditions
        {
            get { return this.GetItems( "/UseCase/MainFlow/Preconditions/Precondition" ); }
        }

        public IItem[] PostConditions
        {
            get { return this.GetItems( "/UseCase/MainFlow/Postconditions/Postcondition" ); }
        }

        public string Name
        {
            get{ return this.doc["UseCase"]["Preface"]["Name"].InnerText; }
            set
            { 
                this.doc["UseCase"]["Preface"]["Name"].InnerText = value;
                this.OnAtomsChanged();
            }
        }

        public string Id
        {
            get{ return this.doc["UseCase"]["Preface"]["ID"].InnerText; }
            set
            {
                this.doc["UseCase"]["Preface"]["ID"].InnerText = value;
                this.OnAtomsChanged();
            }
        }

        public string Summary
        {
            get{ return this.doc["UseCase"]["Preface"]["Summary"].InnerText; }
            set
            {
                this.doc["UseCase"]["Preface"]["Summary"].InnerText = value;
                this.OnAtomsChanged();
            }
        }

        public IElement[] Elements
        {
            get{ return this.GetElements(); }
        }

        public void AddActor( string actorId )
        {
            this.AddItem( "/UseCase/Preface/Actors", "ActorID", actorId );
            this.OnActorsChanged();
        }

        public void AddPrecondition( string precondition )
        {
            this.AddItem( "/UseCase/MainFlow/Preconditions", "Precondition", precondition );
            this.OnPreConditionsChanged();
        }

        public void AddPostcondition( string postcondition )
        {
            this.AddItem( "/UseCase/MainFlow/Postconditions", "Postcondition", postcondition );
            this.OnPostConditionsChanged();
        }

        public void RemoveActor( IItem actor )
        {
            this.RemoveItem( actor );
            this.OnActorsChanged();
        }

        public void RemovePrecondition( IItem pc )
        {
            this.RemoveItem( pc );
            this.OnPreConditionsChanged();
        }

        public void RemovePostcondition( IItem postcondition )
        {
            this.RemoveItem( postcondition );
            this.OnPostConditionsChanged();
        }


        public void AppendElement( IElement element )
        {
            NodeCreatorVisitor visitor = new NodeCreatorVisitor( this.doc );
            element.AcceptVisitor( visitor );

            XmlNode node = this.doc["UseCase"]["MainFlow"]["FlowElements"].AppendChild( 
                this.doc.CreateElement( "FlowElement" ) ).AppendChild( visitor.Node );

            this.OnElementsChanged();
        }

        public void RemoveElement( IElement element )
        {
            XmlNode node = this.doc["UseCase"]["MainFlow"]["FlowElements"].
                SelectSingleNode( string.Format("FlowElement[Step={0}]", element.Text ) );
            this.doc["UseCase"]["MainFlow"]["FlowElements"].RemoveChild( node );            
            
            this.OnElementsChanged();
        }

        public void MoveElementBefore( IItem element, IItem other )
        {
            XmlNode node = ((Item)element).Node;
            XmlNode parent = node.ParentNode;
            parent.RemoveChild( node );
            parent.InsertBefore( node, ((Item)other).Node );

            this.OnElementsChanged();
        }

        public void MoveElementAfter( IItem element, IItem other )
        {
            XmlNode node = ((Item)element).Node;
            XmlNode parent = node.ParentNode;
            parent.RemoveChild( node );
            parent.InsertAfter( node, ((Item)other).Node );
        }

        public void Save( string filename, string xsl )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat( @"<?xml version='1.0'?>
<?xml-stylesheet type='text/xsl' href='{0}'?>{1}", xsl, Environment.NewLine );
            this.FormatXml( this.doc.DocumentElement, builder, 0 );

            using( StreamWriter writer = new StreamWriter( filename ) )
                writer.Write( builder.ToString() );
        }

        protected virtual void OnAtomsChanged()
        {
            if ( this.AtomsChanged != null )
                this.AtomsChanged( this, EventArgs.Empty );

        }

        protected virtual void OnElementsChanged( )
        {
            if ( this.ElementsChanged != null )
                this.ElementsChanged( this, EventArgs.Empty );
        }


        protected virtual void OnActorsChanged()
        {
            if ( this.ActorsChanged != null )
                this.ActorsChanged( this, EventArgs.Empty );
        }

        protected virtual void OnPreConditionsChanged( )
        {
            if ( this.PreConditionsChanged != null )
                this.PreConditionsChanged( this, EventArgs.Empty );
        }

        protected virtual void OnPostConditionsChanged( )
        {
            if ( this.PostConditionsChanged != null )
                this.PostConditionsChanged( this, EventArgs.Empty);
        }

        protected virtual void OnChanged( object sender, System.EventArgs e )
        {
            if ( this.Changed != null )
                this.Changed( sender, e );
        }

        private void SetupEventHandlers()
        {
            this.ActorsChanged += new EventHandler( this.OnChanged );
            this.PreConditionsChanged += new EventHandler( this.OnChanged );
            this.PostConditionsChanged += new EventHandler( this.OnChanged );
            this.ElementsChanged += new EventHandler( this.OnChanged );
            this.AtomsChanged += new EventHandler( this.OnChanged );
        }

        private IElement[] GetElements()
        {
            XmlNodeList list = this.doc.SelectNodes( 
                "/UseCase/MainFlow/FlowElements/FlowElement/*" );
            ArrayList elements = new ArrayList();
            foreach( XmlNode node in list )
            {
                FlowElement element = null;
                switch( node.Name )
                {
                    case "Step":
                        element = new StepElement( node.InnerText );
                        break;
                    case "Include":
                        element = new IncludeElement( node.Attributes["useCaseID"].InnerText );
                        break;
                    default:
                        throw new Exception( "SHould not happen" );
                }

                element.Node = node;     
                elements.Add( element );
            }

            return (IElement[])elements.ToArray( typeof(IElement) );
        }

        private void AddItem( string parentPath, string nodeName, string value )
        {
            XmlNode parent = this.doc.SelectSingleNode( parentPath );
            XmlNode node = this.doc.CreateElement( nodeName );
            node.InnerText = value;

            parent.AppendChild( node );
        }

        private void RemoveItem( IItem item )
        {            
            XmlNode node = ((Item)item).Node;
            node.ParentNode.RemoveChild( node );
        }

        private IItem[] GetItems( string xpath )
        {
            XmlNodeList nodes = this.doc.SelectNodes( xpath );
            ArrayList arr = new ArrayList();
            foreach( XmlNode node in nodes )
                arr.Add( new Item( node.InnerText, node ) );
            
            return (IItem[])arr.ToArray( typeof(IItem) );
        }

        private void PopulateDocument()
        {
            string[] names = typeof(XmlViewForm).Assembly.GetManifestResourceNames();
            
            this.doc.Load( 
                typeof( XmlViewForm ).Assembly.GetManifestResourceStream(
                "UseCase.XmlSkeleton.txt" ) );
                
        }

        private void FormatXml( XmlNode node, StringBuilder text, int level )
        {
            if ( node is XmlWhitespace )
                return;

            const int INDENT = 4;
            text.AppendFormat( "{0}<{1}{2}>", new String( ' ', INDENT*level), node.Name,
                this.FormatAttributes( node ) );

            if ( (node.ChildNodes.Count == 1 && node.ChildNodes[0] is XmlText) ||
                node.ChildNodes.Count == 0 )
            {
                text.AppendFormat( "{0}</{1}>{2}", node.InnerText, node.Name, 
                    Environment.NewLine );
            }
            else
            {
                text.Append( System.Environment.NewLine );
                foreach( XmlNode child in node.ChildNodes )
                    FormatXml( child, text, level + 1 );
                text.AppendFormat( "{0}</{1}>{2}", 
                    new String( ' ', INDENT*level), node.Name, Environment.NewLine );
            }           

        }

        private string FormatAttributes( XmlNode node )
        {
            StringBuilder builder = new StringBuilder();
            foreach( XmlAttribute attr in node.Attributes )
                builder.AppendFormat( " {0}='{1}'", attr.Name, attr.Value );
            
            return builder.ToString();
        }


        private class NodeCreatorVisitor : IElementVisitor
        {
            public NodeCreatorVisitor( XmlDocument doc )
            {
                this.doc = doc;
            }

            public void VisitStepElement( StepElement element)
            {
                this.node = doc.CreateElement( "Step" );
                node.InnerText = element.Text;
            }
            public void VisitIncludeElement( IncludeElement element)
            {
                this.node = doc.CreateElement( "Include" );
                XmlAttribute attr = doc.CreateAttribute( "useCaseID" );
                attr.Value = element.Text;
                this.node.Attributes.Append( attr );            
            }

            public XmlNode Node
            {
                get{ return this.node; }
            }

            private XmlNode node;
            private XmlDocument doc;
            
        }

        private XmlDocument doc;
	}
}
