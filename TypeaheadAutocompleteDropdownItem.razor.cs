using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace DM2BD.Typeahead
{
    public partial class TypeaheadAutocompleteDropdownItem<ItemType> : ComponentBase
    {
        [Parameter]
        public Typeahead<ItemType> Parent { get; set; }
        [Parameter]
        public int MyIndex { get; set; }
        [Parameter]
        public ItemType Self { get; set; }



        public string HoveredCSS
        {
            get => Parent.CurrentIndex == MyIndex ? "background-color: deepskyblue;" : "";
        }

        public string DefaultItemStyles 
        {
            get => Parent.DisableDefaultItemStyles ? "" : "border: 1px solid black; cursor: pointer;";
        }

        public void OnClick() 
        {
            Parent.SearchParameter = Parent.Selector(this.Self); 
            Parent.Search(); 
            Parent.CurrentIndex = 0;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}