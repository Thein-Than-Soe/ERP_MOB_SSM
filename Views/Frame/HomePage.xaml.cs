using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.Data;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Models.Frame;
using CS.ERP_MOB.Services.SYS;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;


namespace CS.ERP_MOB.Views.Frame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentView
    {
        ContentView mContentView = new ContentView();
        ModelRoute mModelRoute;
        private int mADindex = 0;
        private int mPromotionindex = 0;
        private int mSeasonalindex = 0;
        private int mStockindex = 0;
        private int mADDiplayCount = 1; //from commom
        private int mPromotioinDiplayCount = 2; //from commom
        private int mSeasonalDiplayCount = 1; //from commom
        private int mStcokDiplayCount = 3; //from commom
        public HomePage()
        {
            try
            {
                InitializeComponent();
                Device.StartTimer(TimeSpan.FromSeconds(10), () =>
                {
                    // called every 10 second
                 //   RotateCarousel();
                    return true; // return true to repeat counting, false to stop timer
                });
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void RotateCarousel()
        {
            try
            {
                #region "AD Carousel"
                if (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD != null && Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD.Count > 0)
                {
                    if (mADindex < Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD.Count)
                    {
                        if ((Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD.Count - mADindex) < mADDiplayCount)//Last list of one circle
                        {
                            mADDiplayCount = (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD.Count - mADindex);
                        }
                        mContentView = new FrmAD(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD.GetRange(mADindex, 1));
                        CvAd.Content = mContentView;
                        mADindex = mADindex + mADDiplayCount;
                    }
                    else
                    {
                        mADindex = 0;
                        mADDiplayCount = 1;//from common
                    }
                    LayoutAD.IsVisible = true;
                }
                else
                {
                    LayoutAD.IsVisible = false;
                }
                #endregion
                #region "Promotion Carousel"
                if (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION != null && Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION.Count > 0)
                {
                    if (mPromotionindex < Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION.Count)
                    {
                        if ((Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION.Count - mPromotionindex) < mPromotioinDiplayCount)//Last list of one circle
                        {
                            mPromotioinDiplayCount = (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION.Count - mPromotionindex);
                        }
                    }
                    else
                    {
                        mPromotionindex = 0;
                        mPromotioinDiplayCount = 3;//from common
                    }
                    mContentView = new FrmPromotion(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION.GetRange(mPromotionindex, mPromotioinDiplayCount), false);
                    LayoutPromotion.IsVisible = true;
                    CvPromotion.Content = mContentView;
                    mPromotionindex = mPromotionindex + mPromotioinDiplayCount;
                }
                else
                {
                    LayoutPromotion.IsVisible = false;
                }
                #endregion
                #region "Seasonl Carousel"
                if (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK != null && Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK.Count > 0)
                {
                    if (mSeasonalindex < Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK.Count)
                    {
                        if ((Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK.Count - mSeasonalindex) < mSeasonalDiplayCount)//Last list of one circle
                        {
                            mSeasonalDiplayCount = (Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK.Count - mSeasonalindex);
                        }
                       
                    }
                    else
                    {
                        mSeasonalindex = 0;
                        mSeasonalDiplayCount = 3;//from common
                    }
                    mContentView = new FrmSeasonal(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK.GetRange(mSeasonalindex, mSeasonalDiplayCount), false);
                    LayoutSeasonal.IsVisible = true;
                    CvSeasonal.Content = mContentView;
                    mSeasonalindex = mSeasonalindex + mSeasonalDiplayCount;
                }
                else
                {
                    LayoutSeasonal.IsVisible = false;
                }
                #endregion
                #region "Stock Carousel"
                if (Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME != null && Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME.Count > 0)
                {
                    if (mStockindex < Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME.Count)
                    {
                        if ((Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME.Count - mStockindex) < mStcokDiplayCount)//Last list of one circle
                        {
                            mStcokDiplayCount = (Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME.Count - mStockindex);
                        }
                       
                    }
                    else
                    {
                        mStockindex = 0;
                        mStcokDiplayCount = 3;//from common
                    }
                    mContentView = new FrmStock(Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME.GetRange(mStockindex, mStcokDiplayCount), false);
                    LayoutStock.IsVisible = true;
                    CvStock.Content = mContentView;
                    mStockindex = mStockindex + mStcokDiplayCount;
                }
                else
                {
                    LayoutStock.IsVisible = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
               throw ex.InnerException;
            }
        }
        private void TgrADSeeMore_Tapped(object sender, EventArgs e)
        {
            try
            {
                //mContentView = new ADPageList(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD);
                mContentView = new FrmAD(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_AD);
                mModelRoute = new ModelRoute(mContentView, "AD","");
                MessagingCenter.Send<Application, ModelRoute>(Application.Current, "ViewChange", mModelRoute);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void TgrPromotionSeeMore_Tapped(object sender, EventArgs e)
        {
            try
            {
                //mContentView = new PromotionListPage(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION, true);
                mContentView = new FrmPromotion(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_PROMOTION, true);
                mModelRoute = new ModelRoute(mContentView, "Promotion","");
                MessagingCenter.Send<Application, ModelRoute>(Application.Current, "ViewChange", mModelRoute);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void TgrSeasonalSeeMore_Tapped(object sender, EventArgs e)
        {
            try
            {
                //mContentView = new SeasonalListPage(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK, true);
                mContentView = new FrmSeasonal(Common.mCommon.JSN_RES_MOBILE_LOGIN.RES_SEASONAL_STOCK, true);
                mModelRoute = new ModelRoute(mContentView, "Seasonal","");
                MessagingCenter.Send<Application, ModelRoute>(Application.Current, "ViewChange", mModelRoute);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void TgrStcokSeeMore_Tapped(object sender, EventArgs e)
        {
            try
            {
                //mContentView = new StockListPage(Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME, true);
                mContentView = new FrmStock(Common.mCommon.JSN_RES_MOBILE_LOGIN.JSN_INVENTRY_STOCK_HOME.RES_STOCK_HOME, true);
                mModelRoute = new ModelRoute(mContentView, "Stock","");
                MessagingCenter.Send<Application, ModelRoute>(Application.Current, "ViewChange", mModelRoute);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

    }
}