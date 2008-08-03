using System;

namespace UseCase
{
    public interface IElementVisitor
    {
        void VisitStepElement( StepElement element );
        void VisitIncludeElement( IncludeElement element );
    }


    public interface IElement : IItem
    {
        string Text
        {
            get;
        }

        string TagName
        {
            get;
        }

        void AcceptVisitor( IElementVisitor visitor );        
           
    }

    public abstract class FlowElement : Item, IElement
    {
        public FlowElement( string text ) : base( text )
        { }

        public abstract void AcceptVisitor( IElementVisitor visitor );
        public abstract string TagName{ get; }

    }


	/// <summary>
	/// Summary description for FlowElement.
	/// </summary>
	public class StepElement : FlowElement
	{
        public StepElement( string text ) : base( text )
        {}

        public override string TagName
        {
            get{ return "Step"; }
        }

        public override void AcceptVisitor( IElementVisitor visitor )
        {
            visitor.VisitStepElement( this );
        }
	}

    public class IncludeElement : FlowElement
    {
        public IncludeElement( string text ) : base( text )
        {}

        public override string TagName
        {
            get{ return "Include"; }
        }

        public override void AcceptVisitor( IElementVisitor visitor )
        {
            visitor.VisitIncludeElement( this );
        }
    }
}
