using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace DM2BD.Typeahead
{
    public partial class Typeahead<ItemType> : ComponentBase
    {
        [Parameter]
        public EventCallback<List<ItemType>> OnAutocomplete { get; set; }
        [Parameter]
        public EventCallback<string> OnSearch { get; set; }
        // if autocomplete was calculated before search was clicked and they share the same
        // searching they'll return the same list so just return the autocomplete list
        // instead of searching.
        [Parameter]
        public bool ResultsAreSame { get; set; }
        [Parameter]
        public bool DisplayAutocompleteDropdown { get; set; }
        [Parameter]
        public RenderFragment<ItemType> AutocompleteDropdownItemTemplate { get; set; }
        [Parameter]
        public RenderFragment SubmitButtonTemplate { get; set; }
        [Parameter]
        public string Placeholder { get; set; }
        [Parameter]
        public Func<ItemType, string> Selector { get; set; }
        [Parameter]
        public Func<string, Task<List<ItemType>>> Filter { get; set; }
        [Parameter]
        public string Text { get; set; }
        private bool _resetAutocomplete;
        [Parameter]
        public bool ResetAutocomplete 
        {
            get => _resetAutocomplete;
            set
            {
                if(_resetAutocomplete != value)
                {
                    _resetAutocomplete = value;
                    SearchParameter = "";
                    Autocomplete();
                    StateHasChanged();
                }
            }
        }
        //[Parameter]
        //public int Top { get; set; }
        [Parameter]
        public bool DisableDefaultItemStyles { get; set; }

        public bool ToggleAutocompleteDropdown { get; set; }



        private List<ItemType> _autocompleteResults;
        public List<ItemType> AutocompleteResults
        {
            get => _autocompleteResults;
            set
            {
                CurrentIndex = 0;
                _autocompleteResults = value;
                StateHasChanged();
            }
        }



        public int CurrentIndex { get; set; }



        private List<ItemType> _searchResults;
        public List<ItemType> SearchResults
        {
            get
            {
                if (ResultsAreSame)
                {
                    return AutocompleteResults;
                }
                else
                {
                    return _searchResults;
                }
            }
            set => _searchResults = value;
        }



        private string _searchParameter = "";
        public string SearchParameter
        {
            get => _searchParameter;
            set
            {
                _searchParameter = value;
                Autocomplete();
            }
        }



        protected override void OnInitialized()
        {
            base.OnInitialized();
            AutocompleteResults = new List<ItemType>();
        }



        public async void Autocomplete()
        {
            AutocompleteResults = await Filter(SearchParameter);
            if (OnAutocomplete.HasDelegate)
            {
                await OnAutocomplete.InvokeAsync(AutocompleteResults);
            }
        }



        public async void Search()
        {
            ToggleAutocompleteDropdown = false;
            if (!ResultsAreSame)
            {
                SearchResults = await Filter(SearchParameter);
                if (OnSearch.HasDelegate)
                {
                    await OnSearch.InvokeAsync(SearchParameter);
                }
            }
        }



        public void FocusIn()
        {
            ToggleAutocompleteDropdown = true;
            StateHasChanged();
        }



        public async void FocusOut()
        {
            await Task.Delay(250);
            ToggleAutocompleteDropdown = false;
            StateHasChanged();
        }

        public void OnKeyDown(KeyboardEventArgs e)
        {
            ToggleAutocompleteDropdown = true;

            if (e.Key == "ArrowUp")
            {
                CurrentIndex--;



                if (CurrentIndex < 0)
                    CurrentIndex = AutocompleteResults.Count - 1;



                StateHasChanged();
            }
            else if (e.Key == "ArrowDown")
            {
                CurrentIndex++;



                if (CurrentIndex == AutocompleteResults.Count)
                    CurrentIndex = 0;



                StateHasChanged();
            }
            else if (e.Key == "Enter")
            {
                if((CurrentIndex >= 0) && (CurrentIndex < AutocompleteResults.Count))
                {
                    ItemType item = AutocompleteResults[CurrentIndex];
                    SearchParameter = Selector(item);

                    Search();
                }
                else
                {
                    string temp = SearchParameter;
                    SearchParameter = "";

                    Search();

                    SearchParameter = temp;
                }   

                

                StateHasChanged();
            }
        }
    }
}
