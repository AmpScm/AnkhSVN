using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;

namespace UseCase
{
	/// <summary>
	/// Summary description for XmlViewForm.
	/// </summary>
	public class XmlViewForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public XmlViewForm( UseCaseModel useCaseModel )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
//            this.useCaseModel.ElementsChanged += 
//                new ModelChangedEventHandler( this.ElementsChanged );

            this.ActorsChanged( null, EventArgs.Empty );
            this.PreConditionsChanged( null, EventArgs.Empty );
            this.PostConditionsChanged( null, EventArgs.Empty );
            this.AtomsChanged( null, EventArgs.Empty );
            this.RefreshView();
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );

            this.useCaseModel.ActorsChanged -= new 
                ModelChangedEventHandler( this.ActorsChanged );
            this.useCaseModel.PreConditionsChanged -= new 
                ModelChangedEventHandler( this.PreConditionsChanged );
            this.useCaseModel.PostConditionsChanged -= 
                new ModelChangedEventHandler( this.PostConditionsChanged );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(448, 371);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "";
            // 
            // XmlViewForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(448, 371);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.textBox});
            this.Name = "XmlViewForm";
            this.Text = "XmlViewForm";
            this.ResumeLayout(false);

        }
		#endregion

        private void ActorsChanged( object sender, EventArgs e )
        {
            XmlDocument doc = this.xmlDocument;
            XmlNode actorsNode = doc.DocumentElement["Preface"]["Actors"];
            actorsNode.RemoveAll();
            foreach( string actor in this.useCaseModel.Actors.Items )
            {
                XmlNode node = actorsNode.AppendChild( 
                    doc.CreateElement( "ActorID" ) );
                node.InnerText = actor;
            }

            this.RefreshView();
        }

        private void PreConditionsChanged( object sender, EventArgs e )
        {
            XmlDocument doc = this.xmlDocument;
            XmlNode preconditionsNode = doc.DocumentElement["MainFlow"]["Preconditions"];
            preconditionsNode.RemoveAll();
            foreach( string condition in this.useCaseModel.PreConditions.Items )
            {

                XmlNode node = preconditionsNode.AppendChild(
                    doc.CreateElement( "Precondition" ) );
                node.InnerText = condition;
            }

            this.RefreshView();
        }

        private void PostConditionsChanged( object sender, EventArgs e )
        {
            XmlDocument doc = this.xmlDocument;
            XmlNode postconditionsNode = doc.DocumentElement["MainFlow"]["Postconditions"];
            postconditionsNode.RemoveAll();
            foreach( string condition in this.useCaseModel.PostConditions.Items )
            {
                XmlNode node = postconditionsNode.AppendChild(
                    doc.CreateElement( "Postcondition" ) );
                node.InnerText = condition;
            }

            this.RefreshView();
        }

        private void AtomsChanged( object sender, EventArgs e )
        {
            XmlNode node = this.xmlDocument.DocumentElement["Preface"];
            node["Name"].InnerText = this.useCaseModel.Name;
            node["ID"].InnerText = this.useCaseModel.Id;
            node["Summary"].InnerText = this.useCaseModel.Summary;
        }

        private void RefreshView()
        {
            this.textBox.Clear();

            StringBuilder text = new StringBuilder();
            this.FormatXml( this.xmlDocument.DocumentElement, text, 0 );
            this.textBox.Text = text.ToString();
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
        private System.Windows.Forms.RichTextBox textBox;
        private XmlDocument xmlDocument;
	}
}
