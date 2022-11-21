using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;

namespace BlazorTipz.Views
{
    public partial class RegisterU
    {
        bool popup;
        bool isLoading = false;
        public string Checker { get; set; }
        bool HasBeenRegisterd { get; set; }
        UserViewmodel CurrentUser { get; set; } = new();
        UserViewmodel UserDto { get; set; } = new();
        UserViewmodel UserTemp { get; set; } = new();
        //Lager en liste
        List<RoleE> UserRoles { get; set; } = new() { RoleE.User, RoleE.Admin};
        List<UserViewmodel> UsersList { get; set; } = new();
        
        //checks if there is a current user
        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                // If a token is found
                (UserViewmodel user, string err) = await _userManager.GetCurrentUser(token);
                if (err != null)
                {
                    //If error, send to login
                    _navigationManager.NavigateTo("/");
                    return;
                }

                CurrentUser = user;
            }
            else
            {
                _navigationManager.NavigateTo("/");
                return;
            }
            UsersList = _userManager.GetRegisterUserList();
        }

        public void GetUsersList()
        {
            UsersList = _userManager.GetRegisterUserList();
        }
        
        public async Task<ActionResult<string?>> RegisterSingleUser()
        {
            await Task.Delay(0);
            string genPass = _userManager.GenerateRandomPassword();
            UserDto.Password = genPass;
            UserViewmodel usToList = UserDto;
            if (HasBeenRegisterd) {
                _userManager.GetRegisterUserList().Clear();
                HasBeenRegisterd = false;
            }
            string ret = _userManager.StageToRegisterList(usToList);
            Checker = ret;
            UserDto = new();
            return ret;
        }

        //Sender request til registerUserSingel
        public async Task<ActionResult<string?>> RegisterUsers()
        {
            (string? err, string? suc) = await _userManager.RegisterMultiple(null);
            if (err != null)
                Checker = err;
            UsersList = _userManager.GetRegisterUserList();
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
        public async Task<ActionResult<string?>> RegisterUsersWExcel()
        {
            (string? err, string? suc) = await _userManager.RegisterMultiple(null);
            if (err != null)
                Checker = err;
            UsersList = _userManager.GetRegisterUserList();
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
            await Task.Delay(0);
            UserTemp = request;
            _userManager.DeleteFromRegisterList(UserTemp.EmploymentId);
            UsersList = _userManager.GetRegisterUserList();
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
                    worksheet.Cell("A" + row).Value = item.ListNum;
                    worksheet.Cell("B" + row).Value = item.EmploymentId;
                    worksheet.Cell("C" + row).Value = item.UserRole.ToString();
                    worksheet.Cell("D" + row).Value = item.Password;
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