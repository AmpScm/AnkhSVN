using System;

namespace UseCase
{
    public interface IElementVisitor
    {
        void VisitFlowElement( IElement element );
    }

    public interface IElement
    {
        string Text
        {
            get;
        }
    }

	/// <summary>
	/// Summary description for FlowElement.
	/// </summary>
	public class FlowElement : IElement
	{
		public FlowElement( string text )
		{
            this.text = text;			
		}

        public string Text
        {
            get{ return this.text; }
        }

        public void AcceptVisitor( IElementVisitor visitor )
        {
            visitor.VisitFlowElement( this );
        }


        private string text;
	}
}
