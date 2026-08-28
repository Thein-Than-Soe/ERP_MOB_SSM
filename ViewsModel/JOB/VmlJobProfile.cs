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
using CS.ERP_MOB.Extensions;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.JOB;
using CS.ERP_MOB.Services.SYS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.JOB
{
    public class VmlJobProfile : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_APPLICANT_DTL mJSN_REQ_APPLICANT_DTL = new JSN_REQ_APPLICANT_DTL();
        public JSN_RES_APPLICANT_PROFILE mJSN_RES_APPLICANT_PROFILE = new JSN_RES_APPLICANT_PROFILE();
        public JSN_REQ_APPLICANT_PROFILE mJSN_REQ_APPLICANT_PROFILE = new JSN_REQ_APPLICANT_PROFILE();

        public REQ_AUTHORIZATION mREQ_AUTHORIZATION = new REQ_AUTHORIZATION();
        public JSN_LOAD_APPLICANT mJSN_LOAD_APPLICANT = new JSN_LOAD_APPLICANT();
        string mRequest = "";
        string mResponse = "";
        public string mUploadFilePath = "";
        string mDAT_APPLICANT_ask = "";

        // request and response models of load applicant
        public JSN_REQ_APPLICANT mJSN_REQ_APPLICANT = new JSN_REQ_APPLICANT();
        public JSN_REQ_APPLICANT_EDUCATION mJSN_REQ_APPLICANT_EDUCATION = new JSN_REQ_APPLICANT_EDUCATION();
        public JSN_REQ_APPLICANT_WORKING_EXPERIENCE mJSN_REQ_APPLICANT_WORKING_EXPERIENCE = new JSN_REQ_APPLICANT_WORKING_EXPERIENCE();
        public JSN_REQ_APPLICANT_CERTIFICATE mJSN_REQ_APPLICANT_CERTIFICATE = new JSN_REQ_APPLICANT_CERTIFICATE();
        public JSN_REQ_APPLICANT_SKILL mJSN_REQ_APPLICANT_SKILL = new JSN_REQ_APPLICANT_SKILL();
        public JSN_REQ_APPLICANT_LANGUAGE_SKILL mJSN_REQ_APPLICANT_LANGUAGE_SKILL = new JSN_REQ_APPLICANT_LANGUAGE_SKILL();
        public JSN_REQ_APPLICANT_SOCIAL_AFFAIRS mJSN_REQ_APPLICANT_SOCIAL_AFFAIRS = new JSN_REQ_APPLICANT_SOCIAL_AFFAIRS();
        public JSN_REQ_APPLICANT_CONTACT mJSN_REQ_APPLICANT_CONTACT = new JSN_REQ_APPLICANT_CONTACT();


        public JSN_APPLICANT_DTL mJSN_APPLICANT = new JSN_APPLICANT_DTL();
        public JSN_APPLICANT_EDUCATION mJSN_APPLICANT_EDUCATION = new JSN_APPLICANT_EDUCATION();
        public JSN_APPLICANT_WORKING_EXPERIENCE mJSN_APPLICANT_WORKING_EXPERIENCE = new JSN_APPLICANT_WORKING_EXPERIENCE();
        public JSN_APPLICANT_CERTIFICATE mJSN_APPLICANT_CERTIFICATE = new JSN_APPLICANT_CERTIFICATE();
        public JSN_APPLICANT_SKILL mJSN_APPLICANT_SKILL = new JSN_APPLICANT_SKILL();
        public JSN_APPLICANT_LANGUAGE_SKILL mJSN_APPLICANT_LANGUAGE_SKILL = new JSN_APPLICANT_LANGUAGE_SKILL();
        public JSN_APPLICANT_SOCIAL_AFFAIRS mJSN_APPLICANT_SOCIAL_AFFAIRS = new JSN_APPLICANT_SOCIAL_AFFAIRS();
        public JSN_APPLICANT_CONTACT mJSN_APPLICANT_CONTACT = new JSN_APPLICANT_CONTACT();
        #endregion

        #region "Contructor"
        public VmlJobProfile()
        {
            //ApplicantEducation = new ObservableCollection<DAT_APPLICANT_EDUCATION>();
            //ApplicantCertificate = new ObservableCollection<DAT_APPLICANT_CERTIFICATE>();
            //ApplicantExperience = new ObservableCollection<DAT_APPLICANT_WORKING_EXPERIENCE>();
            //ApplicantLanguage = new ObservableCollection<DAT_APPLICANT_LANGUAGE_SKILL>();
            //ApplicantSkill = new ObservableCollection<DAT_APPLICANT_SKILL>();
            //ApplicantSocialAffairs = new ObservableCollection<DAT_APPLICANT_SOCIAL_AFFAIRS>();
            //ApplicantContact = new ObservableCollection<DAT_APPLICANT_CONTACT>();
            //Applicant = new ObservableCollection<DAT_APPLICANT>();
            //Person = new ObservableCollection<DAT_PERSON>();
        }
        #endregion

        #region "Display View"

        private bool mIsRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return mIsRefreshing;
            }
            set
            {
                mIsRefreshing = value;
                NotifyPropertyChanged("IsRefreshing");
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
        // for count fields in profile
        private string _contactCount = "0";
        public string Contact_count
        {
            get => _contactCount;
            set
            {
                if (_contactCount != value)
                {
                    _contactCount = value;
                    NotifyPropertyChanged(nameof(Contact_count));
                }
            }
        }

        private string _educationCount = "0";
        public string Education_count
        {
            get => _educationCount;
            set
            {
                if (_educationCount != value)
                {
                    _educationCount = value;
                    NotifyPropertyChanged(nameof(Education_count));
                }
            }
        }

        private string _experienceCount = "0";
        public string Experience_count
        {
            get => _experienceCount;
            set
            {
                if (_experienceCount != value)
                {
                    _experienceCount = value;
                    NotifyPropertyChanged(nameof(Experience_count));
                }
            }
        }

        private string _skillCount = "0";
        public string Skill_count
        {
            get => _skillCount;
            set
            {
                if (_skillCount != value)
                {
                    _skillCount = value;
                    NotifyPropertyChanged(nameof(Skill_count));
                }
            }
        }

        private string _languageCount = "0";
        public string Language_count
        {
            get => _languageCount;
            set
            {
                if (_languageCount != value)
                {
                    _languageCount = value;
                    NotifyPropertyChanged(nameof(Language_count));
                }
            }
        }

        private string _socialCount = "0";
        public string Social_count
        {
            get => _socialCount;
            set
            {
                if (_socialCount != value)
                {
                    _socialCount = value;
                    NotifyPropertyChanged(nameof(Social_count));
                }
            }
        }

        private string _certificateCount = "0";
        public string Certificate_count
        {
            get => _certificateCount;
            set
            {
                if (_certificateCount != value)
                {
                    _certificateCount = value;
                    NotifyPropertyChanged(nameof(Certificate_count));
                }
            }
        }

        #endregion

        #region "Data Tab"


        // Country, state, city
        private RES_COUNTRY_DTL _selectedCountry;
        public RES_COUNTRY_DTL SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    NotifyPropertyChanged(nameof(SelectedCountry));
                    // reset downstream when parent changes
                    SelectedState = null;
                    SelectedCity = null;
                }
            }
        }

        private RES_STATE_DTL _selectedState;
        public RES_STATE_DTL SelectedState
        {
            get => _selectedState;
            set
            {
                if (_selectedState != value)
                {
                    _selectedState = value;
                    NotifyPropertyChanged(nameof(SelectedState));
                    // reset city when state changes
                    SelectedCity = null;
                }
            }
        }

        private RES_CITY _selectedCity;
        public RES_CITY SelectedCity
        {
            get => _selectedCity;
            set { if (_selectedCity != value) { _selectedCity = value; NotifyPropertyChanged(nameof(SelectedCity)); } }
        }


        //User info

        public JSN_RES_APPLICANT_PROFILE mApplicantProfileData;
        public JSN_RES_APPLICANT_PROFILE ApplicantProfileData
        {
            get { return mApplicantProfileData; }
            set { mApplicantProfileData = value; NotifyPropertyChanged("ApplicantProfileData"); }
        }

        private ObservableCollection<DAT_APPLICANT> _applicant = new();
        public ObservableCollection<DAT_APPLICANT> Applicant
        {
            get => _applicant;
            set
            {
                if (_applicant != value)
                {
                    _applicant = value;
                    NotifyPropertyChanged(nameof(Applicant));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_CONTACT> _applicantContact = new();
        public ObservableCollection<DAT_APPLICANT_CONTACT> ApplicantContact
        {
            get => _applicantContact;
            set
            {
                if (_applicantContact != value)
                {
                    _applicantContact = value;
                    NotifyPropertyChanged(nameof(ApplicantContact));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_EDUCATION> _applicantEducation = new();
        public ObservableCollection<DAT_APPLICANT_EDUCATION> ApplicantEducation
        {
            get => _applicantEducation;
            set
            {
                if (_applicantEducation != value)
                {
                    _applicantEducation = value;
                    NotifyPropertyChanged(nameof(ApplicantEducation));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_CERTIFICATE> _applicantCertificate = new();
        public ObservableCollection<DAT_APPLICANT_CERTIFICATE> ApplicantCertificate
        {
            get => _applicantCertificate;
            set
            {
                if (_applicantCertificate != value)
                {
                    _applicantCertificate = value;
                    NotifyPropertyChanged(nameof(ApplicantCertificate));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_WORKING_EXPERIENCE> _applicantExperience = new();
        public ObservableCollection<DAT_APPLICANT_WORKING_EXPERIENCE> ApplicantExperience
        {
            get => _applicantExperience;
            set
            {
                if (_applicantExperience != value)
                {
                    _applicantExperience = value;
                    NotifyPropertyChanged(nameof(ApplicantExperience));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_LANGUAGE_SKILL> _applicantLanguage = new();
        public ObservableCollection<DAT_APPLICANT_LANGUAGE_SKILL> ApplicantLanguage
        {
            get => _applicantLanguage;
            set
            {
                if (_applicantLanguage != value)
                {
                    _applicantLanguage = value;
                    NotifyPropertyChanged(nameof(ApplicantLanguage));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_SKILL> _applicantSkill = new();
        public ObservableCollection<DAT_APPLICANT_SKILL> ApplicantSkill
        {
            get => _applicantSkill;
            set
            {
                if (_applicantSkill != value)
                {
                    _applicantSkill = value;
                    NotifyPropertyChanged(nameof(ApplicantSkill));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_SOCIAL_AFFAIRS> _applicantSocialAffairs = new();
        public ObservableCollection<DAT_APPLICANT_SOCIAL_AFFAIRS> ApplicantSocialAffairs
        {
            get => _applicantSocialAffairs;
            set
            {
                if (_applicantSocialAffairs != value)
                {
                    _applicantSocialAffairs = value;
                    NotifyPropertyChanged(nameof(ApplicantSocialAffairs));
                }
            }
        }

        private ObservableCollection<DAT_APPLICANT_TRAINING> _applicantTraining = new();
        public ObservableCollection<DAT_APPLICANT_TRAINING> ApplicantTraining
        {
            get => _applicantTraining;
            set
            {
                if (_applicantTraining != value)
                {
                    _applicantTraining = value;
                    NotifyPropertyChanged(nameof(ApplicantTraining));
                }
            }
        }

        #endregion

        #region "Load of Dropdown Data Tab"


        public List<RES_CURRENCY> mCurrencyList;
        public List<RES_CURRENCY> CurrencyList
        {
            get { return mCurrencyList; }
            set { mCurrencyList = value; NotifyPropertyChanged("CurrencyList"); }
        }
        public List<RES_NATIONALITY> mNationalityList;
        public List<RES_NATIONALITY> NationalityList
        {
            get { return mNationalityList; }
            set { mNationalityList = value; NotifyPropertyChanged("NationalityList"); }
        }
        public List<DAT_APPLICANT_AVAILABILITY> mAvailabilityList;
        public List<DAT_APPLICANT_AVAILABILITY> AvailabilityList
        {
            get { return mAvailabilityList; }
            set { mAvailabilityList = value; NotifyPropertyChanged("AvailabilityList"); }
        }
        public List<ERP.PL.SYS.DAT.RES_GENDER> mGenderList;
        public List<ERP.PL.SYS.DAT.RES_GENDER> GenderList
        {
            get { return mGenderList; }
            set { mGenderList = value; NotifyPropertyChanged("GenderList"); }
        }
        public List<RES_COUNTRY_DTL> mCountryList;
        public List<RES_COUNTRY_DTL> CountryList
        {
            get { return mCountryList; }
            set { mCountryList = value; NotifyPropertyChanged("CountryList"); }
        }


        public List<DAT_EDUCATION_LEVEL> mEducationLevelList;
        public List<DAT_EDUCATION_LEVEL> EducationLevelList
        {
            get { return mEducationLevelList; }
            set { mEducationLevelList = value; NotifyPropertyChanged("EducationLevelList"); }
        }
        public List<DAT_CERTIFICATE_TYPE> mCertificateTypeList;
        public List<DAT_CERTIFICATE_TYPE> CertificateTypeList
        {
            get { return mCertificateTypeList; }
            set { mCertificateTypeList = value; NotifyPropertyChanged("CertificateTypeList"); }
        }

        public List<DAT_CONTACT_TYPE> mContactTypeList;
        public List<DAT_CONTACT_TYPE> ContactTypeList
        {
            get { return mContactTypeList; }
            set { mContactTypeList = value; NotifyPropertyChanged("ContactTypeList"); }
        }

        public List<DAT_DIVISION> mDivisionList;
        public List<DAT_DIVISION> DivisionList
        {
            get { return mDivisionList; }
            set { mDivisionList = value; NotifyPropertyChanged("DivisionList"); }
        }

        public List<DAT_LANGUAGE_LEVEL> mLanguageLevelList;
        public List<DAT_LANGUAGE_LEVEL> LanguageLevelList
        {
            get { return mLanguageLevelList; }
            set { mLanguageLevelList = value; NotifyPropertyChanged("LanguageLevelList"); }
        }

        public List<DAT_DESIGNATION> mDesignationList;
        public List<DAT_DESIGNATION> DesignationList
        {
            get { return mDesignationList; }
            set { mDesignationList = value; NotifyPropertyChanged("DesignationList"); }
        }

        public List<DAT_BUSINESS_TYPE> mBusinessTypeList;
        public List<DAT_BUSINESS_TYPE> BusinessTypeList
        {
            get { return mBusinessTypeList; }
            set { mBusinessTypeList = value; NotifyPropertyChanged("BusinessTypeList"); }
        }

        public List<DAT_SKILL_LEVEL> mSkillLevelList;
        public List<DAT_SKILL_LEVEL> SkillLevelList
        {
            get { return mSkillLevelList; }
            set { mSkillLevelList = value; NotifyPropertyChanged("SkillLevelList"); }
        }
        public List<DAT_EMPLOYMENT_TYPE> mEmploymentTypeList;
        public List<DAT_EMPLOYMENT_TYPE> EmploymentTypeList
        {
            get { return mEmploymentTypeList; }
            set { mEmploymentTypeList = value; NotifyPropertyChanged("EmploymentTypeList"); }
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

                var uploadFolderName = Job_UploadFolder.job_Applicant;

                string response = await Job_Service.UploadImageToServer(uploadFolderName, "photo", result.FileName, imageBytes);

                if (response != null)
                {

                    mUploadFilePath = "/uploads" + Job_UploadFolder.job_Applicant + "/" + response;
                    ProfileImagePath = Job_Service.getUploadURL() + mUploadFilePath;

                }
                else
                {
                    mUploadFilePath = Applicant[0].Profile;
                    ProfileImagePath = Job_Service.getUploadURL() + mUploadFilePath;
                }

            }

            catch (Exception ex)

            {

                throw new Exception(ex.Message, ex);

            }

        });

        private ICommand mRefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (mRefreshCommand == null)
                {
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
                return mRefreshCommand;
            }
        }
        #endregion

        #region "Method"
        // Bind data tab for profile page
        private void bindAllData(JSN_RES_APPLICANT_PROFILE argJSN_RES_APPLICANT_PROFILE)
        {
            try
            {
                if (argJSN_RES_APPLICANT_PROFILE != null)
                {
                    //Count binding
                    Contact_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_CONTACT.Count.ToString();
                    Education_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_EDUCATION.Count.ToString();
                    Experience_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_WORKING_EXPERIENCE.Count.ToString();
                    Skill_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_SKILL.Count.ToString();
                    Language_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_LANGUAGE_SKILL.Count.ToString();
                    Social_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_SOCIAL_AFFAIRS.Count.ToString();
                    Certificate_count = argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_CERTIFICATE.Count.ToString();

                    //bind img url include bind
                    if (argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT.Count > 0)
                    {
                        formatApplicant(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT[0]);
                    }

                    //format date
                    formatExperience(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_WORKING_EXPERIENCE);
                    formatEducation(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_EDUCATION);
                    formatCertificate(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_CERTIFICATE);

                    //bind all data with lists
                    ApplicantProfileData = argJSN_RES_APPLICANT_PROFILE;
                    
                    Applicant = new ObservableCollection<DAT_APPLICANT>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT);
                    ApplicantContact = new ObservableCollection<DAT_APPLICANT_CONTACT>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_CONTACT);
                    ApplicantCertificate = new ObservableCollection<DAT_APPLICANT_CERTIFICATE>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_CERTIFICATE);
                    ApplicantEducation = new ObservableCollection<DAT_APPLICANT_EDUCATION>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_EDUCATION);
                    ApplicantExperience = new ObservableCollection<DAT_APPLICANT_WORKING_EXPERIENCE>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_WORKING_EXPERIENCE);
                    ApplicantLanguage = new ObservableCollection<DAT_APPLICANT_LANGUAGE_SKILL>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_LANGUAGE_SKILL);
                    ApplicantSkill = new ObservableCollection<DAT_APPLICANT_SKILL>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_SKILL);
                    ApplicantSocialAffairs = new ObservableCollection<DAT_APPLICANT_SOCIAL_AFFAIRS>(argJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT_SOCIAL_AFFAIRS);

                }
                else
                {
                    ApplicantProfileData = new JSN_RES_APPLICANT_PROFILE();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void formatApplicant(DAT_APPLICANT argDAT_APPLICANT) { 
            try
            {
                if (argDAT_APPLICANT != null)
                {
                    //bind img url
                    DAT_APPLICANT DAT_APPLICANT = argDAT_APPLICANT;
                    DAT_APPLICANT.DOB = Utility.getDateTimeString(DAT_APPLICANT.DOB).ToString();
                    ProfileImagePath = Job_Service.getUploadURL() + argDAT_APPLICANT.Profile;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private void formatExperience(List<DAT_APPLICANT_WORKING_EXPERIENCE> argDAT_EXP) { 
            try
            {
                if (argDAT_EXP != null && argDAT_EXP.Count > 0)
                {
                    //format date
                    foreach (DAT_APPLICANT_WORKING_EXPERIENCE l_DAT_EXP in argDAT_EXP.Where(e => e != null))
                    {
                        l_DAT_EXP.SD = Utility.getDateTimeString(l_DAT_EXP.SD).ToString();
                        l_DAT_EXP.ED = Utility.getDateTimeString(l_DAT_EXP.ED).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void formatEducation(List<DAT_APPLICANT_EDUCATION> argDAT_EDU) { 
            try
            {
                if (argDAT_EDU != null)
                {
                    //format date
                    foreach (DAT_APPLICANT_EDUCATION l_DAT_EDU in argDAT_EDU.Where(e => e != null))
                    {
                        l_DAT_EDU.SD = Utility.getDateTimeString(l_DAT_EDU.SD).ToString();
                        l_DAT_EDU.ED = Utility.getDateTimeString(l_DAT_EDU.ED).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void formatCertificate(List<DAT_APPLICANT_CERTIFICATE> argDAT_CERTIFICATE) { 
            try
            {
                if (argDAT_CERTIFICATE != null)
                {
                    //format date
                    foreach (DAT_APPLICANT_CERTIFICATE l_DAT_CERT in argDAT_CERTIFICATE.Where(e => e != null))
                    {
                        l_DAT_CERT.SD = Utility.getDateTimeString(l_DAT_CERT.SD).ToString();
                        l_DAT_CERT.ED = Utility.getDateTimeString(l_DAT_CERT.ED).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //bind country
        public void bindCountry(List<RES_COUNTRY_DTL> argRES_COUNTRY_DTL_LST)
        {
            try
            {
                if (argRES_COUNTRY_DTL_LST != null && argRES_COUNTRY_DTL_LST.Count > 0)
                {
                    CountryList = argRES_COUNTRY_DTL_LST;
                }
                else
                {
                    CountryList = new List<RES_COUNTRY_DTL>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        // Bind data for load api

        public void bindCurrency(List<RES_CURRENCY> argRES_CURRENCY_LST)
        {
            try
            {
                if (argRES_CURRENCY_LST != null && argRES_CURRENCY_LST.Count > 0)
                {
                    CurrencyList = argRES_CURRENCY_LST;
                }
                else
                {
                    CurrencyList = new List<RES_CURRENCY>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindNationality(List<RES_NATIONALITY> argRES_NATIONALITY_LST)
        {
            try
            {
                if (argRES_NATIONALITY_LST != null && argRES_NATIONALITY_LST.Count > 0)
                {
                    NationalityList = argRES_NATIONALITY_LST;
                }
                else
                {
                    NationalityList = new List<RES_NATIONALITY>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindGender(List<ERP.PL.SYS.DAT.RES_GENDER> argRES_GENDER_LST)
        {
            try
            {
                if (argRES_GENDER_LST != null && argRES_GENDER_LST.Count > 0)
                {
                    GenderList = argRES_GENDER_LST;
                }
                else
                {
                    GenderList = new List<ERP.PL.SYS.DAT.RES_GENDER>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindAvailability(List<DAT_APPLICANT_AVAILABILITY> argDAT_APPLICANT_AVAILABILITY_LST)
        {
            try
            {
                if (argDAT_APPLICANT_AVAILABILITY_LST != null && argDAT_APPLICANT_AVAILABILITY_LST.Count > 0)
                {
                    AvailabilityList = argDAT_APPLICANT_AVAILABILITY_LST;
                }
                else
                {
                    AvailabilityList = new List<DAT_APPLICANT_AVAILABILITY>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindEducationLevel(List<DAT_EDUCATION_LEVEL> argDAT_EDUCATION_LEVEL_LST)
        {
            try
            {
                if (argDAT_EDUCATION_LEVEL_LST != null && argDAT_EDUCATION_LEVEL_LST.Count > 0)
                {
                    EducationLevelList = argDAT_EDUCATION_LEVEL_LST;
                }
                else
                {
                    EducationLevelList = new List<DAT_EDUCATION_LEVEL>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindCertificateType(List<DAT_CERTIFICATE_TYPE> argDAT_CERTIFICATE_TYPE_LST)
        {
            try
            {
                if (argDAT_CERTIFICATE_TYPE_LST != null && argDAT_CERTIFICATE_TYPE_LST.Count > 0)
                {
                    CertificateTypeList = argDAT_CERTIFICATE_TYPE_LST;
                }
                else
                {
                    CertificateTypeList = new List<DAT_CERTIFICATE_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindContactType(List<DAT_CONTACT_TYPE> argDAT_CONTACT_TYPE_LST)
        {
            try
            {
                if (argDAT_CONTACT_TYPE_LST != null && argDAT_CONTACT_TYPE_LST.Count > 0)
                {
                    ContactTypeList = argDAT_CONTACT_TYPE_LST;
                }
                else
                {
                    ContactTypeList = new List<DAT_CONTACT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindDivision(List<DAT_DIVISION> argDAT_DIVISION_LST)
        {
            try
            {
                if (argDAT_DIVISION_LST != null && argDAT_DIVISION_LST.Count > 0)
                {
                    DivisionList = argDAT_DIVISION_LST;
                }
                else
                {
                    DivisionList = new List<DAT_DIVISION>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindLanguageLevel(List<DAT_LANGUAGE_LEVEL> argDAT_LANGUAGE_LEVEL_LST)
        {
            try
            {
                if (argDAT_LANGUAGE_LEVEL_LST != null && argDAT_LANGUAGE_LEVEL_LST.Count > 0)
                {
                    LanguageLevelList = argDAT_LANGUAGE_LEVEL_LST;
                }
                else
                {
                    LanguageLevelList = new List<DAT_LANGUAGE_LEVEL>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindDesignation(List<DAT_DESIGNATION> argDAT_DESIGNATION_LST)
        {
            try
            {
                if (argDAT_DESIGNATION_LST != null && argDAT_DESIGNATION_LST.Count > 0)
                {
                    DesignationList = argDAT_DESIGNATION_LST;
                }
                else
                {
                    DesignationList = new List<DAT_DESIGNATION>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindBusinessType(List<DAT_BUSINESS_TYPE> argDAT_BUSINESS_TYPE_LST)
        {
            try
            {
                if (argDAT_BUSINESS_TYPE_LST != null && argDAT_BUSINESS_TYPE_LST.Count > 0)
                {
                    BusinessTypeList = argDAT_BUSINESS_TYPE_LST;
                }
                else
                {
                    BusinessTypeList = new List<DAT_BUSINESS_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void bindSkillLevel(List<DAT_SKILL_LEVEL> argDAT_SKILL_LEVEL_LST)
        {
            try
            {
                if (argDAT_SKILL_LEVEL_LST != null && argDAT_SKILL_LEVEL_LST.Count > 0)
                {
                    SkillLevelList = argDAT_SKILL_LEVEL_LST;
                }
                else
                {
                    SkillLevelList = new List<DAT_SKILL_LEVEL>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void bindEmploymentType(List<DAT_EMPLOYMENT_TYPE> argDAT_EMPLOYMENT_TYPE_LST)
        {
            try
            {
                if (argDAT_EMPLOYMENT_TYPE_LST != null && argDAT_EMPLOYMENT_TYPE_LST.Count > 0)
                {
                    EmploymentTypeList = argDAT_EMPLOYMENT_TYPE_LST;
                }
                else
                {
                    EmploymentTypeList = new List<DAT_EMPLOYMENT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #endregion


        #region "Web Service Api"
        public async Task getApplicantDetail()
        {
            try
            {
                Utility.openLoader();
                this.mJSN_REQ_APPLICANT_DTL.DAT_PERSON = new List<DAT_PERSON> { new DAT_PERSON() };
                this.mJSN_REQ_APPLICANT_DTL.DAT_APPLICANT = new List<DAT_APPLICANT> { new DAT_APPLICANT() };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_DTL);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsgetApplicantDtl);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_RES_APPLICANT_PROFILE = JsonConvert.DeserializeObject<JSN_RES_APPLICANT_PROFILE>(mResponse);
                    if (this.mJSN_RES_APPLICANT_PROFILE.Message.Code == "7")
                    {
                        if (this.mJSN_RES_APPLICANT_PROFILE.RES_USER_LST != null)
                        {
                            //Get applicant ask here
                            if (mJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT != null && mJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT.Count > 0)
                            {
                                mDAT_APPLICANT_ask = mJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT[0].Ask;
                            }
                            bindAllData(this.mJSN_RES_APPLICANT_PROFILE);
                            //WeakReferenceMessenger.Default.Send("Successfully call get api!");
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send("There is no data to display!");
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_APPLICANT_PROFILE.Message.Message);
                    }

                    Utility.closeLoader();
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                Utility.closeLoader();
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task loadApplicant()
        {
            try
            {
                Utility.openLoader();
                this.mREQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                string l_Request = JsonConvert.SerializeObject(mREQ_AUTHORIZATION);
                string l_Response = await Job_Service.ApiCall(l_Request, Job_Name.wsLoadApplicant);
                if (!string.IsNullOrEmpty(l_Response))
                {
                    this.mJSN_LOAD_APPLICANT = JsonConvert.DeserializeObject<JSN_LOAD_APPLICANT>(l_Response);
                    if (this.mJSN_LOAD_APPLICANT.Message.Code == "7")
                    {
                        // Data binding here 
                        //bindAllLoadData(this.mJSN_LOAD_APPLICANT);
                        bindCountry(this.mJSN_LOAD_APPLICANT.RES_COUNTRY_DTL);
                        bindGender(this.mJSN_LOAD_APPLICANT.RES_GENDER);
                        bindCurrency(this.mJSN_LOAD_APPLICANT.RES_CURRENCY);
                        bindNationality(this.mJSN_LOAD_APPLICANT.RES_NATIONALITY);
                        bindAvailability(this.mJSN_LOAD_APPLICANT.DAT_APPLICANT_AVAILABILITY);
                        bindEducationLevel(this.mJSN_LOAD_APPLICANT.DAT_EDUCATION_LEVEL);
                        bindCertificateType(this.mJSN_LOAD_APPLICANT.DAT_CERTIFICATE_TYPE);
                        bindContactType(this.mJSN_LOAD_APPLICANT.DAT_CONTACT_TYPE);
                        bindDivision(this.mJSN_LOAD_APPLICANT.DAT_DIVISION);
                        bindLanguageLevel(this.mJSN_LOAD_APPLICANT.DAT_LANGUAGE_LEVEL);
                        bindDesignation(this.mJSN_LOAD_APPLICANT.DAT_DESIGNATION);
                        bindBusinessType(this.mJSN_LOAD_APPLICANT.DAT_BUSINESS_TYPE);
                        bindSkillLevel(this.mJSN_LOAD_APPLICANT.DAT_SKILL_LEVEL);
                        bindEmploymentType(this.mJSN_LOAD_APPLICANT.DAT_EMPLOYMENT_TYPE);

                        //WeakReferenceMessenger.Default.Send("Successfully call load api!");
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_APPLICANT_PROFILE.Message.Message);
                    }

                    Utility.closeLoader();
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                Utility.closeLoader();
                throw new Exception(ex.Message, ex);
            }
        }

        //For update in profile page (Need to fix)
        public async Task saveApplicantProfile(DAT_APPLICANT argDAT_APPLICANT)
        {
            try
            {
                argDAT_APPLICANT.Profile = mUploadFilePath;
                mJSN_REQ_APPLICANT_PROFILE.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_PROFILE.DAT_APPLICANT = new List<DAT_APPLICANT> { argDAT_APPLICANT };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_PROFILE);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicant);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_RES_APPLICANT_PROFILE = JsonConvert.DeserializeObject<JSN_RES_APPLICANT_PROFILE>(mResponse);
                    if (mJSN_RES_APPLICANT_PROFILE.Message.Code == "7")
                    {
                        //For save btn update
                        DAT_APPLICANT l_DAT_APPLICANT = mJSN_RES_APPLICANT_PROFILE.DAT_APPLICANT[0];
                        ProfileImagePath = Job_Service.getUploadURL() + l_DAT_APPLICANT.Profile;
                        NotifyPropertyChanged("ProfileImagePath");

                        Applicant.RemoveAt(0);       // Remove original item at index i
                        Applicant.Insert(0, l_DAT_APPLICANT);
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully Save!", "OK");
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_APPLICANT_PROFILE.Message.Message);
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
        public async Task saveApplicantEducation(DAT_APPLICANT_EDUCATION argDAT_APPLICANT_EDUCATION)
        {
            try
            {
                argDAT_APPLICANT_EDUCATION.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_EDUCATION.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_EDUCATION.DAT_APPLICANT_EDUCATION = new List<DAT_APPLICANT_EDUCATION> { argDAT_APPLICANT_EDUCATION };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_EDUCATION);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantEducation);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_EDUCATION = JsonConvert.DeserializeObject<JSN_APPLICANT_EDUCATION>(mResponse);
                    if (mJSN_APPLICANT_EDUCATION.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_EDUCATION.StatusAsk == "1")
                        {
                            // Response assign
                            DAT_APPLICANT_EDUCATION l_DAT_APPLICANT_EDUCATION = mJSN_APPLICANT_EDUCATION.DAT_APPLICANT_EDUCATION[0];
                            l_DAT_APPLICANT_EDUCATION.SD = Utility.getDateTimeString(l_DAT_APPLICANT_EDUCATION.SD).ToString();
                            l_DAT_APPLICANT_EDUCATION.ED = Utility.getDateTimeString(l_DAT_APPLICANT_EDUCATION.ED).ToString();

                            //For save btn new
                            if (argDAT_APPLICANT_EDUCATION.StatusAsk == "1" && argDAT_APPLICANT_EDUCATION.Ask == "0")
                            {
                                ApplicantEducation.Add(l_DAT_APPLICANT_EDUCATION); // Add new item

                                WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_EDUCATION.Message.Message);
                            }
                            //For save btn update
                            else
                            {
                                for (int i = 0; i < ApplicantEducation.Count; i++)
                                {
                                    if (ApplicantEducation[i].Ask == l_DAT_APPLICANT_EDUCATION.Ask)
                                    {
                                        ApplicantEducation.RemoveAt(i);       // Remove original item at index i
                                        ApplicantEducation.Insert(i, l_DAT_APPLICANT_EDUCATION);   // Insert updated item at same index

                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_EDUCATION.Message.Message);
                                        break; // Quit loop after successful match
                                    }
                                }
                            }
                        }

                        // For delete btn
                        else if (argDAT_APPLICANT_EDUCATION.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantEducation.Count; i++)
                            {
                                if (ApplicantEducation[i].Ask == argDAT_APPLICANT_EDUCATION.Ask)
                                {
                                    ApplicantEducation.RemoveAt(i);       // Remove original item at index i
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_EDUCATION.Message.Message);
                                    break; // Quit loop after successful match
                                }
                            }
                        }
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
                        Education_count = ApplicantEducation.Count.ToString();

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

        public async Task saveApplicantExperience(DAT_APPLICANT_WORKING_EXPERIENCE argDAT_APPLICANT_WORKING_EXPERIENCE)
        {
            try
            {
                argDAT_APPLICANT_WORKING_EXPERIENCE.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_WORKING_EXPERIENCE.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_WORKING_EXPERIENCE.DAT_APPLICANT_WORKING_EXPERIENCE = new List<DAT_APPLICANT_WORKING_EXPERIENCE> { argDAT_APPLICANT_WORKING_EXPERIENCE };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_WORKING_EXPERIENCE);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantWorkingExperience);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_WORKING_EXPERIENCE = JsonConvert.DeserializeObject<JSN_APPLICANT_WORKING_EXPERIENCE>(mResponse);
                    if (mJSN_APPLICANT_WORKING_EXPERIENCE.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_WORKING_EXPERIENCE.StatusAsk == "1")
                        {
                            // Response assign
                            DAT_APPLICANT_WORKING_EXPERIENCE l_DAT_APPLICANT_WORKING_EXPERIENCE = mJSN_APPLICANT_WORKING_EXPERIENCE.DAT_APPLICANT_WORKING_EXPERIENCE[0];
                            l_DAT_APPLICANT_WORKING_EXPERIENCE.SD = Utility.getDateTimeString(l_DAT_APPLICANT_WORKING_EXPERIENCE.SD).ToString();
                            l_DAT_APPLICANT_WORKING_EXPERIENCE.ED = Utility.getDateTimeString(l_DAT_APPLICANT_WORKING_EXPERIENCE.ED).ToString();

                            //For save btn new
                            if (argDAT_APPLICANT_WORKING_EXPERIENCE.StatusAsk == "1" && argDAT_APPLICANT_WORKING_EXPERIENCE.Ask == "0")
                            {
                                ApplicantExperience.Add(l_DAT_APPLICANT_WORKING_EXPERIENCE); // Add new item
                            }
                            //For save btn update
                            else
                            {
                                for (int i = 0; i < ApplicantExperience.Count; i++)
                                {
                                    if (ApplicantExperience[i].Ask == l_DAT_APPLICANT_WORKING_EXPERIENCE.Ask)
                                    {
                                        ApplicantExperience.RemoveAt(i);       // Remove original item at index i
                                        ApplicantExperience.Insert(i, l_DAT_APPLICANT_WORKING_EXPERIENCE);   // Insert updated item at same index

                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_WORKING_EXPERIENCE.Message.Message);
                                        break; // Quit loop after successful match
                                    }
                                }
                            }
                        }
                        // For delete btn
                        else if (argDAT_APPLICANT_WORKING_EXPERIENCE.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantExperience.Count; i++)
                            {
                                if (ApplicantExperience[i].Ask == argDAT_APPLICANT_WORKING_EXPERIENCE.Ask)
                                {
                                    ApplicantExperience.RemoveAt(i);       // Remove original item at index i
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_WORKING_EXPERIENCE.Message.Message);
                                    break; // Quit loop after successful match
                                }
                            }
                        }
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
                        Experience_count = ApplicantExperience.Count.ToString();
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

        public async Task saveApplicantCertificate(DAT_APPLICANT_CERTIFICATE argDAT_APPLICANT_CERTIFICATE)
        {
            try
            {
                argDAT_APPLICANT_CERTIFICATE.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_CERTIFICATE.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_CERTIFICATE.DAT_APPLICANT_CERTIFICATE = new List<DAT_APPLICANT_CERTIFICATE> { argDAT_APPLICANT_CERTIFICATE };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_CERTIFICATE);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantCertificate);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_CERTIFICATE = JsonConvert.DeserializeObject<JSN_APPLICANT_CERTIFICATE>(mResponse);
                    if (mJSN_APPLICANT_CERTIFICATE.Message.Code == "7")
                    {
                        
                        if (argDAT_APPLICANT_CERTIFICATE.StatusAsk == "1")
                        {
                            DAT_APPLICANT_CERTIFICATE l_DAT_APPLICANT_CERTIFICATE = mJSN_APPLICANT_CERTIFICATE.DAT_APPLICANT_CERTIFICATE[0];
                            l_DAT_APPLICANT_CERTIFICATE.SD = Utility.getDateTimeString(l_DAT_APPLICANT_CERTIFICATE.SD).ToString();
                            l_DAT_APPLICANT_CERTIFICATE.ED = Utility.getDateTimeString(l_DAT_APPLICANT_CERTIFICATE.ED).ToString();
                            if (argDAT_APPLICANT_CERTIFICATE.StatusAsk == "1" && argDAT_APPLICANT_CERTIFICATE.Ask == "0")
                            {
                                ApplicantCertificate.Add(l_DAT_APPLICANT_CERTIFICATE);
                            }
                            else
                            {
                                for (int i = 0; i < ApplicantCertificate.Count; i++)
                                {
                                    if (ApplicantCertificate[i].Ask == l_DAT_APPLICANT_CERTIFICATE.Ask)
                                    {
                                        ApplicantCertificate.RemoveAt(i);
                                        ApplicantCertificate.Insert(i, l_DAT_APPLICANT_CERTIFICATE);
                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_CERTIFICATE.Message.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (argDAT_APPLICANT_CERTIFICATE.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantCertificate.Count; i++)
                            {
                                if (ApplicantCertificate[i].Ask == argDAT_APPLICANT_CERTIFICATE.Ask)
                                {
                                    ApplicantCertificate.RemoveAt(i);
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_CERTIFICATE.Message.Message);
                                    break;
                                }
                            }
                        }
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
                        Certificate_count = ApplicantCertificate.Count.ToString();
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

        public async Task saveApplicantSkill(DAT_APPLICANT_SKILL argDAT_APPLICANT_SKILL)
        {
            try
            {
                argDAT_APPLICANT_SKILL.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_SKILL.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_SKILL.DAT_APPLICANT_SKILL = new List<DAT_APPLICANT_SKILL> { argDAT_APPLICANT_SKILL };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_SKILL);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantSkill);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_SKILL = JsonConvert.DeserializeObject<JSN_APPLICANT_SKILL>(mResponse);
                    if (mJSN_APPLICANT_SKILL.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_SKILL.StatusAsk == "1")
                        {
                            DAT_APPLICANT_SKILL l_DAT_APPLICANT_SKILL = mJSN_APPLICANT_SKILL.DAT_APPLICANT_SKILL[0];
                            if (argDAT_APPLICANT_SKILL.Ask == "0")
                            {
                                ApplicantSkill.Add(l_DAT_APPLICANT_SKILL);
                            }
                            else
                            {
                                for (int i = 0; i < ApplicantSkill.Count; i++)
                                {
                                    if (ApplicantSkill[i].Ask == l_DAT_APPLICANT_SKILL.Ask)
                                    {
                                        ApplicantSkill.RemoveAt(i);
                                        ApplicantSkill.Insert(i, l_DAT_APPLICANT_SKILL);
                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_SKILL.Message.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (argDAT_APPLICANT_SKILL.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantSkill.Count; i++)
                            {
                                if (ApplicantSkill[i].Ask == argDAT_APPLICANT_SKILL.Ask)
                                {
                                    ApplicantSkill.RemoveAt(i);
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_SKILL.Message.Message);
                                    break;
                                }
                            }
                        }
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
                        Skill_count = ApplicantSkill.Count.ToString();

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

        public async Task saveApplicantLanguage(DAT_APPLICANT_LANGUAGE_SKILL argDAT_APPLICANT_LANGUAGE_SKILL)
        {
            try
            {
                argDAT_APPLICANT_LANGUAGE_SKILL.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_LANGUAGE_SKILL.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_LANGUAGE_SKILL.DAT_APPLICANT_LANGUAGE_SKILL = new List<DAT_APPLICANT_LANGUAGE_SKILL> { argDAT_APPLICANT_LANGUAGE_SKILL };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_LANGUAGE_SKILL);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantLanguageSkill);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_LANGUAGE_SKILL = JsonConvert.DeserializeObject<JSN_APPLICANT_LANGUAGE_SKILL>(mResponse);
                    if (mJSN_APPLICANT_LANGUAGE_SKILL.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_LANGUAGE_SKILL.StatusAsk == "1")
                        {
                            DAT_APPLICANT_LANGUAGE_SKILL l_DAT_APPLICANT_LANGUAGE_SKILL = mJSN_APPLICANT_LANGUAGE_SKILL.DAT_APPLICANT_LANGUAGE_SKILL[0];

                            if (argDAT_APPLICANT_LANGUAGE_SKILL.StatusAsk == "1" && argDAT_APPLICANT_LANGUAGE_SKILL.Ask == "0")
                            {
                                ApplicantLanguage.Add(l_DAT_APPLICANT_LANGUAGE_SKILL);
                            }
                            else
                            {
                                for (int i = 0; i < ApplicantLanguage.Count; i++)
                                {
                                    if (ApplicantLanguage[i].Ask == l_DAT_APPLICANT_LANGUAGE_SKILL.Ask)
                                    {
                                        ApplicantLanguage.RemoveAt(i);
                                        ApplicantLanguage.Insert(i, l_DAT_APPLICANT_LANGUAGE_SKILL);
                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_LANGUAGE_SKILL.Message.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (argDAT_APPLICANT_LANGUAGE_SKILL.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantLanguage.Count; i++)
                            {
                                if (ApplicantLanguage[i].Ask == argDAT_APPLICANT_LANGUAGE_SKILL.Ask)
                                {
                                    ApplicantLanguage.RemoveAt(i);
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_LANGUAGE_SKILL.Message.Message);
                                    break;
                                }
                            }
                        }

                        Language_count = ApplicantLanguage.Count.ToString();
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
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

        public async Task saveApplicantSocialAffairs(DAT_APPLICANT_SOCIAL_AFFAIRS argDAT_APPLICANT_SOCIAL_AFFAIRS)
        {
            try
            {
                argDAT_APPLICANT_SOCIAL_AFFAIRS.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_SOCIAL_AFFAIRS.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_SOCIAL_AFFAIRS.DAT_APPLICANT_SOCIAL_AFFAIRS = new List<DAT_APPLICANT_SOCIAL_AFFAIRS> { argDAT_APPLICANT_SOCIAL_AFFAIRS };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_SOCIAL_AFFAIRS);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantSocialAffairs);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_SOCIAL_AFFAIRS = JsonConvert.DeserializeObject<JSN_APPLICANT_SOCIAL_AFFAIRS>(mResponse);
                    if (mJSN_APPLICANT_SOCIAL_AFFAIRS.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_SOCIAL_AFFAIRS.StatusAsk == "1")
                        {
                            DAT_APPLICANT_SOCIAL_AFFAIRS l_DAT_APPLICANT_SOCIAL_AFFAIRS = mJSN_APPLICANT_SOCIAL_AFFAIRS.DAT_APPLICANT_SOCIAL_AFFAIRS[0];

                            if (argDAT_APPLICANT_SOCIAL_AFFAIRS.StatusAsk == "1" && argDAT_APPLICANT_SOCIAL_AFFAIRS.Ask == "0")
                            {
                                ApplicantSocialAffairs.Add(l_DAT_APPLICANT_SOCIAL_AFFAIRS);
                            }
                            else
                            {
                                for (int i = 0; i < ApplicantSocialAffairs.Count; i++)
                                {
                                    if (ApplicantSocialAffairs[i].Ask == l_DAT_APPLICANT_SOCIAL_AFFAIRS.Ask)
                                    {
                                        ApplicantSocialAffairs.RemoveAt(i);
                                        ApplicantSocialAffairs.Insert(i, l_DAT_APPLICANT_SOCIAL_AFFAIRS);
                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_SOCIAL_AFFAIRS.Message.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (argDAT_APPLICANT_SOCIAL_AFFAIRS.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantSocialAffairs.Count; i++)
                            {
                                if (ApplicantSocialAffairs[i].Ask == argDAT_APPLICANT_SOCIAL_AFFAIRS.Ask)
                                {
                                    ApplicantSocialAffairs.RemoveAt(i);
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_SOCIAL_AFFAIRS.Message.Message);
                                    break;
                                }
                            }
                        }

                        Social_count = ApplicantSocialAffairs.Count.ToString();
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
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

        public async Task saveApplicantContact(DAT_APPLICANT_CONTACT argDAT_APPLICANT_CONTACT)
        {
            try
            {
                argDAT_APPLICANT_CONTACT.ApplicantAsk = mDAT_APPLICANT_ask;
                mJSN_REQ_APPLICANT_CONTACT.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_CONTACT.DAT_APPLICANT_CONTACT = new List<DAT_APPLICANT_CONTACT> { argDAT_APPLICANT_CONTACT };
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_CONTACT);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantContact);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_APPLICANT_CONTACT = JsonConvert.DeserializeObject<JSN_APPLICANT_CONTACT>(mResponse);
                    if (mJSN_APPLICANT_CONTACT.Message.Code == "7")
                    {
                        if (argDAT_APPLICANT_CONTACT.StatusAsk == "1")
                        {
                            DAT_APPLICANT_CONTACT l_DAT_APPLICANT_CONTACT = mJSN_APPLICANT_CONTACT.DAT_APPLICANT_CONTACT[0];

                            if (argDAT_APPLICANT_CONTACT.StatusAsk == "1" && argDAT_APPLICANT_CONTACT.Ask == "0")
                            {
                                ApplicantContact.Add(l_DAT_APPLICANT_CONTACT);
                            }
                            else
                            {
                                for (int i = 0; i < ApplicantContact.Count; i++)
                                {
                                    if (ApplicantContact[i].Ask == l_DAT_APPLICANT_CONTACT.Ask)
                                    {
                                        ApplicantContact.RemoveAt(i);
                                        ApplicantContact.Insert(i, l_DAT_APPLICANT_CONTACT);
                                        WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_CONTACT.Message.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (argDAT_APPLICANT_CONTACT.StatusAsk == "6")
                        {
                            for (int i = 0; i < ApplicantContact.Count; i++)
                            {
                                if (ApplicantContact[i].Ask == argDAT_APPLICANT_CONTACT.Ask)
                                {
                                    ApplicantContact.RemoveAt(i);
                                    WeakReferenceMessenger.Default.Send(this.mJSN_APPLICANT_CONTACT.Message.Message);
                                    break;
                                }
                            }
                        }

                        Contact_count = ApplicantContact.Count.ToString();
                        await Application.Current.MainPage.DisplayAlert("Success", "Successfully updated", "OK");
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
