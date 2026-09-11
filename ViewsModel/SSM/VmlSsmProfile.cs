using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.REQ;
using CS.ERP.PL.HCM.RES;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.PMA_API.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP.PL.SYS.RES;
using CS.ERP_MOB.Extensions;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.SSM;
using CS.ERP_MOB.Services.SYS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using CS.ERP_MOB.DB;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmProfile : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_USER_LST mJSN_REQ_USER_LST = new JSN_REQ_USER_LST();
        public JSN_USER mJSN_USER = new JSN_USER();


        string mRequest = "";
        string mResponse = "";
        public string mUploadFilePath = "";
        #endregion

        #region "Contructor"
        public VmlSsmProfile()
        {
            try
            {
                //if (Common.mCommon.mDbUser_LST.Count > 0)
                //{
                //    foreach (DbUser l_user in Common.mCommon.mDbUser_LST)
                //    {
                //        if (l_user.UserAsk.ToLower() != Common.mCommon.User.UserAsk
                //            && l_user.UserID.ToLower() != "guest")
                //        {
                //            DbUser.Add(l_user);
                //        }
                //    }
                //}

                // No valid user found
                //if (DbUser.Count <= 0)
                //{
                //    System.Diagnostics.Debug.WriteLine(
                //        "VmlSsmProfile: No valid user found.");

                //    return;
                //}

                // Get current user
                RES_USER currentUser = Common.mCommon.User;

                mDbUser.UserID = currentUser.UserID;
                mDbUser.UserName_0_255 = currentUser.UserDescription;
                mDbUser.UserEmail = currentUser.UserEmail;
                mDbUser.UserPhone = currentUser.UserPhone;
                mDbUser.UserProfile = currentUser.ProfilePicture;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"VmlSsmProfile constructor error: {ex}");

                throw;
            }
        }
        #endregion

        #region "Display View"
        private RES_USER_LST _mDbUser = new RES_USER_LST();

        public RES_USER_LST mDbUser
        {
            get => _mDbUser;
            set
            {
                if (_mDbUser == value)
                    return;

                _mDbUser = value;
                NotifyPropertyChanged(nameof(mDbUser));
            }
        }
        #endregion

        #region "Count fields"
        // for image update in profile
        private string _imagePath;
        public string ProfileImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    NotifyPropertyChanged(nameof(ProfileImagePath));
                }
            }
        }
        
        #endregion

        
        #region "Commands"
        public ICommand PickImageCommand => new Command(async () => 
        {

            try

            {
                var result = await FilePicker.Default.PickAsync(new PickOptions

                {

                    PickerTitle = "Select a photo",

                    FileTypes = FilePickerFileType.Images

                });

                if (result == null)

                    return;

                var stream = await result.OpenReadAsync();

                using var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);

                byte[] imageBytes = memoryStream.ToArray();

                var uploadFolderName = Sys_UploadFolder.sys_User;

                string response = await Sys_Service.UploadImageToServer(uploadFolderName, "photo", result.FileName, imageBytes);

                if (response != null)
                {

                    mUploadFilePath = "/uploads" + Sys_UploadFolder.sys_User + "/" + response;
                    ProfileImagePath = Sys_Service.getUploadURL() + mUploadFilePath;

                }
                else
                {
                    mUploadFilePath = Common.mCommon.User.ProfilePicture;
                }

            }

            catch (Exception ex)

            {

                throw new Exception(ex.Message, ex);

            }

        });

        #endregion

        #region "Method"
        
        #endregion


        #region "Web Service Api"
        public async Task saveUser()
        {
            try
            {
                mDbUser.UserProfile = mUploadFilePath;
                mJSN_REQ_USER_LST.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_USER_LST.RES_USER_LST = new List<RES_USER_LST> { mDbUser };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_USER_LST);
                mResponse = await Sys_Service.ApiCall(mRequest, Sys_Name.wsSaveUser);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_USER = JsonConvert.DeserializeObject<JSN_USER>(mResponse);
                    if (mJSN_USER.Message.Code == "7")
                    {
                        //For save btn update
                        ProfileImagePath = mJSN_USER.RES_USER_LST[0].UserProfile;
                        NotifyPropertyChanged("ProfileImagePath");

                        WeakReferenceMessenger.Default.Send(this.mJSN_USER.Message.Message);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

    }

}
