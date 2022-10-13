using BlazorTipz.Data;
using Microsoft.AspNetCore.Mvc;
using BlazorTipz.ViewModels.User;
using Radzen.Blazor;
using System;
using Radzen;

namespace BlazorTipz.Views
{
    public partial class RegisterU
    {
        bool popup;
        bool isLoading = false;
        public string Checker { get; set; }

        bool passwordVisible = true;
        UserViewmodel userDto = new UserViewmodel();
        UserViewmodel userTemp = new UserViewmodel();
        //Lager en liste
        List<RoleE> roles = new List<RoleE>{RoleE.User, RoleE.Admin};
        List<UserViewmodel> UsersList;

        RadzenDataGrid<UserViewmodel> grid;

        //checks if there is a current user
        protected override async Task OnInitializedAsync()
        {
            //checks if currentUser is null
            var CurrentUser = _userM.getCurrentUser();
            if (CurrentUser == null)
            {
                _navigationManager.NavigateTo("/");
            }
            else
            {
                UsersList = _userM.getRegisterUserList();
            }
        }

        public async Task<ActionResult<string?>> registerUser()
        {
            string genPass = _userM.generatePassword();
            userDto.password = genPass;
            UserViewmodel usToList = userDto;
            string ret = _userM.stageToRegisterList(usToList);
            Checker = ret;
            userDto = new UserViewmodel();
            await grid.Reload();
            return ret;
        }

        //Sender resuest til registerUserSingel
        public async Task<ActionResult<string>> RegisterUsers()
        {
            (string err, string suc) = await _userM.registerMultiple(null);
            Checker = err;
            UsersList = _userM.getRegisterUserList();
            if (suc != null)
            {
                Checker = suc;
                return suc;
            }

            return err;
        }

        //Delete user from list
        public async Task<ActionResult<string>> DeleteUser(UserViewmodel request)
        {
            userTemp = request;
            _userM.deleteFromRegisterList(userTemp.employmentId);
            UsersList = _userM.getRegisterUserList();
            await grid.Reload();
            //Break foreach loop
            return "User deleted";
        }
    }
}