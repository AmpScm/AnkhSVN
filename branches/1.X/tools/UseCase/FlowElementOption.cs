using System;

namespace UseCase
{
    public interface IFlowElementOption
    {
        string LabelText
        {
            get;
        }

        IElement CreateElement( string text );
    }
	/// <summary>
	/// Summary description for FlowElementOption.
	/// </summary>
	public class StepElementOption : IFlowElementOption
	{
        public string LabelText
        {
            get{ return "Step"; }
        }

        public IElement CreateElement( string text )
        {
            return new StepElement( text );
        }
	}

    public class IncludeElementOption : IFlowElementOption
    {
        public string LabelText
        {
            get{ return "UC ID"; }
        }

        public IElement CreateElement( string text )
        {
            return new IncludeElement( text );
        }
    }
}
