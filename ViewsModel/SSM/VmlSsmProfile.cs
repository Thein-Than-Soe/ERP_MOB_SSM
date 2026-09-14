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
using CS.ERP.PL.SSM.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP.PL.SYS.RES;
using CS.ERP_MOB.DB;
using CS.ERP_MOB.Extensions;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.SSM;
using CS.ERP_MOB.Services.SYS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmProfile : BaseViewModel
    {
        #region "Declaring"
        //get
        JSN_PROFILE_DETAIL mJSN_PROFILE_DETAIL = new JSN_PROFILE_DETAIL();

        //save
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
                getProfileDetail();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(  $"VmlSsmProfile constructor error: {ex}");
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
        //public ICommand PickImageCommand => new Command(async () =>
        //{
        //    try
        //    {
        //        var result = await FilePicker.Default.PickAsync(new PickOptions
        //        {
        //            PickerTitle = "Select a photo",
        //            FileTypes = FilePickerFileType.Images
        //        });

        //        if (result == null)
        //            return;

        //        // Keep original filename for API
        //        mUploadFilePath = result.FileName;

        //        // Create a safe local filename
        //        string extension = Path.GetExtension(result.FileName);

        //        if (string.IsNullOrWhiteSpace(extension))
        //            extension = ".jpg";

        //        string localFilePath = Path.Combine(
        //            FileSystem.CacheDirectory,
        //            $"profile_{Guid.NewGuid():N}{extension}");

        //        await using (Stream sourceStream = await result.OpenReadAsync())
        //        await using (FileStream destinationStream = File.Create(localFilePath))
        //        {
        //            await sourceStream.CopyToAsync(destinationStream);
        //        }

        //        // Display selected image
        //        ProfileImagePath = localFilePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //    }
        //});

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

                // Read selected image
                await using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);

                byte[] imageBytes = memoryStream.ToArray();

                // Upload image to server
                var uploadFolderName = Sys_UploadFolder.sys_User;

                string response = await Sys_Service.UploadImageToServer(
                    uploadFolderName,
                    "photo",
                    result.FileName,
                    imageBytes);

                if (!string.IsNullOrWhiteSpace(response))
                {
                    mUploadFilePath = "/uploads" + Sys_UploadFolder.sys_User + "/" + response;
                    ProfileImagePath =  Sys_Service.getUploadURL() + mUploadFilePath;
                }
                else
                {
                    mUploadFilePath = mDbUser.UserProfile;
                    ProfileImagePath = Sys_Service.getUploadURL() + mUploadFilePath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        });
        #endregion

        #region "Method"

        #endregion


        #region "Web Service Api"
        public async Task getProfileDetail()
        {
            try
            {
                Utility.openLoader();

                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Sys_Service.ApiCall(mRequest, Sys_Name.wsgetProfileDetail);
                if (!string.IsNullOrWhiteSpace(mResponse))
                {
                    this.mJSN_PROFILE_DETAIL = JsonConvert.DeserializeObject<JSN_PROFILE_DETAIL>(mResponse);
                    if (mJSN_PROFILE_DETAIL.Message.Code == "7")
                    {
                        //For save btn update
                        mDbUser = mJSN_PROFILE_DETAIL.RES_USER_LST;
                        ProfileImagePath = Sys_Service.getUploadURL() + mDbUser.UserProfile;

                        WeakReferenceMessenger.Default.Send(this.mJSN_PROFILE_DETAIL.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }

        public async Task saveUser()
        {
            try
            {
                Utility.openLoader();

                if (!string.IsNullOrWhiteSpace(mUploadFilePath)) {
                    mDbUser.UserProfile = mUploadFilePath;
                }
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
                        mDbUser = mJSN_USER.RES_USER_LST[0];
                        if (!string.IsNullOrWhiteSpace(mUploadFilePath) && mJSN_USER.RES_USER_LST != null && mJSN_USER.RES_USER_LST.Count > 0) { ProfileImagePath = Sys_Service.getUploadURL() + mDbUser.UserProfile; }
                        
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
            finally
            {
                Utility.closeLoader();
            }
        }

        #endregion

    }

}
