using System;
using System.Collections;

namespace UseCase
{
    public delegate void ModelChangedEventHandler( object sender, EventArgs e );
	/// <summary>
	/// Summary description for UseCaseModel.
	/// </summary>
    public class UseCaseModel
    {
        public event ModelChangedEventHandler ActorsChanged;
        public event ModelChangedEventHandler PreConditionsChanged;
        public event ModelChangedEventHandler PostConditionsChanged;
        public event ModelChangedEventHandler ElementsChanged;
        public event ModelChangedEventHandler AtomsChanged;

        public UseCaseModel()
        {
            this.actors = new ItemCollection();
            this.actors.Changed += new ModelChangedEventHandler( this.OnActorsChanged );

            this.preConditions = new ItemCollection();
            this.preConditions.Changed += new ModelChangedEventHandler( 
                this.OnPreConditionsChanged );

            this.postConditions = new ItemCollection();
            this.postConditions.Changed += new ModelChangedEventHandler( 
                this.OnPostConditionsChanged );

            this.elements = new ArrayList();           
        }

        public ItemCollection Actors
        {
            get { return this.actors; }
        }

        public ItemCollection PreConditions
        {
            get { return this.preConditions; }
        }

        public ItemCollection PostConditions
        {
            get { return this.postConditions; }
        }

        public string Name
        {
            get{ return this.name; }
            set
            { 
                this.name = value;
                this.OnAtomsChanged();
            }
        }

        public string Id
        {
            get{ return this.id; }
            set
            {
                this.id = value;
                this.OnAtomsChanged();
            }
        }

        public string Summary
        {
            get{ return this.summary; }
            set
            {
                this.summary = value;
                this.OnAtomsChanged();
            }
        }

        public IElement[] Elements
        {
            get{ return (IElement[])this.elements.ToArray( typeof(IElement) ); }
        }

        public void AppendElement( IElement element )
        {
            this.elements.Add( element );
            this.OnElementsChanged();
        }

        public void RemoveElement( IElement element )
        {
            this.elements.Remove( element );
            this.OnElementsChanged();
        }

        public void MoveElementBefore( IElement element, IElement other )
        {
            this.elements.Remove( element );
            int index = this.elements.IndexOf( other );
            this.elements.Insert( index, element );

            this.OnElementsChanged();
        }

        public void MoveElementAfter( IElement element, IElement other )
        {
            this.elements.Remove( element );
            int index = this.elements.IndexOf( other );
            this.elements.Insert( index + 1, element );

            this.OnElementsChanged();
        }

  
        


        public class ItemCollection
        {
            public event ModelChangedEventHandler Changed;

            public void Add( string item )
            {
                this.list.Add( item );
                if ( this.Changed != null )
                    this.Changed( null, EventArgs.Empty );

            }

            public void Delete( string item )
            {
                this.list.Remove( item );
                if ( this.Changed != null )
                    this.Changed( null, EventArgs.Empty );
            }

            public string[] Items
            {
                get{ return (string[])this.list.ToArray( typeof(string) ); }
            }

            private ArrayList list = new ArrayList();
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


        protected virtual void OnActorsChanged( object sender, EventArgs e )
        {
            if ( this.ActorsChanged != null )
                this.ActorsChanged( this, e );
        }

        protected virtual void OnPreConditionsChanged( object sender, EventArgs e )
        {
            if ( this.PreConditionsChanged != null )
                this.PreConditionsChanged( this, e );
        }

        protected virtual void OnPostConditionsChanged( object sender, EventArgs e )
        {
            if ( this.PostConditionsChanged != null )
                this.PostConditionsChanged( this, e );
        }


        private ItemCollection actors;
        private ItemCollection preConditions;
        private ItemCollection postConditions;

        private ArrayList elements;
        private string name = "";
        private string id = "";
        private string summary = "";
	}
}
