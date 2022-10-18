using BlazorTipz.Data;
using Microsoft.AspNetCore.Mvc;
using BlazorTipz.ViewModels.User;
using System.Data;
using ClosedXML.Excel;
using Microsoft.JSInterop;

namespace BlazorTipz.Views
{
    public partial class RegisterU
    {
        bool popup;
        bool isLoading = false;
        public string Checker { get; set; }
        bool HasBeenRegisterd { get; set; }

        bool passwordVisible = true;
        UserViewmodel userDto = new UserViewmodel();
        UserViewmodel userTemp = new UserViewmodel();
        //Lager en liste
        List<RoleE> roles = new List<RoleE>{RoleE.User, RoleE.Admin};
        List<UserViewmodel> UsersList;
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

        public void getUsersList()
        {
            UsersList = _userM.getRegisterUserList();
        }

        public async Task<ActionResult<string?>> registerUser()
        {
            string genPass = _userM.generatePassword();
            userDto.password = genPass;
            UserViewmodel usToList = userDto;
            if (HasBeenRegisterd) {
                _userM.getRegisterUserList().Clear();
                HasBeenRegisterd = false;
            }
            string ret = _userM.stageToRegisterList(usToList);
            Checker = ret;
            userDto = new UserViewmodel();
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
                DialogService.Close();
                HasBeenRegisterd = true;
                return suc;
            }

            return err;
        }

        //Sender resuest til registerUserSingel og eksporter en excel fil
        public async Task<ActionResult<string>> RegisterUsersWExcel()
        {
            (string err, string suc) = await _userM.registerMultiple(null);
            Checker = err;
            UsersList = _userM.getRegisterUserList();
            if (suc != null)
            {
                await DownloadExcelDocument();
                Checker = suc;
                DialogService.Close();
                HasBeenRegisterd = true;
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
            //Break foreach loop
            return "User deleted";
        }


        public async Task DownloadExcelDocument()
        {
            //Get current date 24hr format
            string date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("UserList");
                worksheet.Cell("A1").Value = "List number";
                worksheet.Cell("B1").Value = "Employment ID";
                worksheet.Cell("C1").Value = "Role";
                worksheet.Cell("D1").Value = "Password";
                //Set width of columns
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 10;
                worksheet.Column(4).Width = 10;

                int row = 2;

                foreach (var item in UsersList)
                {
                    worksheet.Cell("A" + row).Value = item.listnum;
                    worksheet.Cell("B" + row).Value = item.employmentId;
                    worksheet.Cell("C" + row).Value = item.role.ToString();
                    worksheet.Cell("D" + row).Value = item.password;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    await JSRuntime.InvokeVoidAsync("saveAsFile",
                    $"UserList_{date}.xlsx", Convert.ToBase64String(content));
                }
            }

        }
    }
}