using System;
using System.Xml;
using System.Text;
using System.IO;

namespace UseCase
{
	/// <summary>
	/// Summary description for XmlModel.
	/// </summary>
	public class XmlModel
	{
        public event EventHandler Changed;

		public XmlModel( UseCaseModel useCaseModel )
		{
            this.xmlDocument = new XmlDocument();
            this.PopulateDocument();

            this.useCaseModel = useCaseModel;
            this.useCaseModel.ActorsChanged += new 
                ModelChangedEventHandler( this.ActorsChanged );
            this.useCaseModel.PreConditionsChanged += new 
                ModelChangedEventHandler( this.PreConditionsChanged );
            this.useCaseModel.PostConditionsChanged += 
                new ModelChangedEventHandler( this.PostConditionsChanged );
            this.useCaseModel.AtomsChanged +=
                new ModelChangedEventHandler( this.AtomsChanged );
            this.useCaseModel.ElementsChanged += 
                new ModelChangedEventHandler( this.ElementsChanged );

            this.ActorsChanged( null, EventArgs.Empty );
            this.PreConditionsChanged( null, EventArgs.Empty );
            this.PostConditionsChanged( null, EventArgs.Empty );
            this.AtomsChanged( null, EventArgs.Empty );
            this.ElementsChanged( null, EventArgs.Empty );
			
		}

        public string Text
        {
            get
            { 
                StringBuilder builder = new StringBuilder();
                this.FormatXml( this.xmlDocument.DocumentElement, builder, 0 );
                return builder.ToString();
            }
        }

        public string Xsl
        {
            get{ return this.xsl; }
            set{ this.xsl = value; }
        }
        
        public void Save( string filename )
        {
            using( StreamWriter writer = new StreamWriter( filename ) )
            {
                writer.WriteLine( "<?xml version=\"1.0\"?>" );
                if ( this.xsl != null )
                    writer.WriteLine( 
                        "<?xml-stylesheet type='text/xsl' href='{0}'?>", this.xsl );

                writer.Write( this.Text );
            }
        }



        protected virtual void OnChanged()
        {
            if ( this.Changed != null )
                this.Changed( this, EventArgs.Empty );
        }

        private void ElementsChanged( object sender, EventArgs e )
        {
            XmlNode parentNode = 
                this.xmlDocument.DocumentElement["MainFlow"]["FlowElements"];
            XmlDocument doc = this.xmlDocument;
            foreach( IElement item in this.useCaseModel.Elements )
            {
                parentNode.AppendChild( doc.CreateElement( "FlowElement" ) ).
                    AppendChild( doc.CreateElement( "Step" ) ).InnerText = item.Text;
            }
                
        }

        private void ActorsChanged( object sender, EventArgs e )
        {
            this.FillCollection( this.xmlDocument.DocumentElement["Preface"]["Actors"],
                this.useCaseModel.Actors.Items, "ActorID" );
        }

        private void PreConditionsChanged( object sender, EventArgs e )
        {
            this.FillCollection( this.xmlDocument.DocumentElement["MainFlow"]["Preconditions"],
                this.useCaseModel.PreConditions.Items, "Precondition" );
        }

        private void PostConditionsChanged( object sender, EventArgs e )
        {
            
            this.FillCollection( this.xmlDocument.DocumentElement["MainFlow"]["Postconditions"],
                this.useCaseModel.PostConditions.Items, "PostCondition" );           
        }

        private void FillCollection( XmlNode parentNode, 
            string[] items, string nodeName )
        {
            XmlDocument doc = this.xmlDocument;
            parentNode.RemoveAll();
            foreach( string item in items )
            {
                XmlNode node = parentNode.AppendChild(
                    doc.CreateElement( nodeName ) );
                node.InnerText = item;
            }

            this.OnChanged();

        }


        private void AtomsChanged( object sender, EventArgs e )
        {
            XmlNode node = this.xmlDocument.DocumentElement["Preface"];
            node["Name"].InnerText = this.useCaseModel.Name;
            node["ID"].InnerText = this.useCaseModel.Id;
            node["Summary"].InnerText = this.useCaseModel.Summary;

            this.OnChanged();
        }

        private void FormatXml( XmlNode node, StringBuilder text, int level )
        {
            if ( node is XmlWhitespace )
                return;

            const int INDENT = 4;
            text.AppendFormat( "{0}<{1}>", new String( ' ', INDENT*level), node.Name );

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

        private void PopulateDocument()
        {
            string[] names = typeof(XmlViewForm).Assembly.GetManifestResourceNames();
            
            this.xmlDocument.Load( 
                typeof( XmlViewForm ).Assembly.GetManifestResourceStream(
                "UseCase.XmlSkeleton.txt" ) );
                
        }



        private UseCaseModel useCaseModel;
        private XmlDocument xmlDocument;
        private string xsl;
	}
}
