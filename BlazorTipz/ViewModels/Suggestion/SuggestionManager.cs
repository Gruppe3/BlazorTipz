using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggestionManager : ISuggestionManager
    {
        private readonly IDbRelay _DBR;
        // Constructor
        public SuggestionManager(IDbRelay DBR)
        {
            _DBR = DBR;
        }

        public async Task<string?> saveSuggestion(SuggViewmodel sugg)
        {
            string err = null;
            if (sugg == null) { err = "No supplied suggestion"; return err; }
            if (sugg.Title == null) { err = "No supplied title"; return err; }
            if (sugg.Description == null) { err = "No supplied description"; return err; }
            if (sugg.OwnerTeam == null) { err = "No supplied owner"; return err; }
            if (sugg.Creator == null) { err = "No supplied creator"; return err; }
            if (sugg.StartDate == null) { err = "No supplied start date"; return err; }

            SuggestionEntity suggEntity = new SuggestionEntity(sugg);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.saveSuggestion(suggEntity);

            return err;
        }
    }
}
