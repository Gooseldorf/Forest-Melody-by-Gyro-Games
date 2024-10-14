using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace CardTD.NoAssembly
{
    public class IAPManager : IDetailedStoreListener
    {
        private static ProductCatalog productCatalog;

        public IStoreController Controller;

        public static IAPManager Link;

        public ProductCatalog ProductCatalog => productCatalog;
        public static bool IsInitialized => Link != null && Link.Controller != null;

        public static async Task InitServices()
        {
            if (Link == null)
            {
                try
                {
                    var options = new InitializationOptions();
                    await UnityServices.InitializeAsync(options);
                    InitAIP();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private static void InitAIP()
        {
            Link = new IAPManager();
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            productCatalog = ProductCatalog.LoadDefaultCatalog();

            foreach (var product in productCatalog.allProducts)
            {
                builder.AddProduct(product.id, product.type);
            }

            UnityPurchasing.Initialize(Link, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Controller = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"OnInitializeFailed: {error.ToString()}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message) { }



        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            //Debug.Log($"BuyProduct {purchaseEvent.purchasedProduct.definition.id}");
            string purchasedItem = purchaseEvent.purchasedProduct.definition.id;
            //Messenger<string>.Broadcast(GameEvents.ProcessPurchase, purchasedItem, MessengerMode.DONT_REQUIRE_LISTENER);
            SO_Scripts.CrystalPack crystalPack = SO_Scripts.DataHolder.Instance.GetCrystalPack(purchasedItem);
            if (crystalPack != null)
                crystalPack.Activate();
            else
                Debug.LogError($"can't find crystal pack by id: " + purchasedItem);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"OnPurchaseFailed: product:{product.definition} failureReason: {failureDescription.reason.ToString()}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }



        public static void BuyProduct(string productId)
        {
            Debug.Log("Try to buy product");
            if (IsInitialized)
            {
                Link.Controller.InitiatePurchase(productId);
            }
            else
                Debug.LogError("IStoreController is not initialized");
        }

        public static string GetProductCost(string productId)
        {
            if (IsInitialized)
            {
                foreach (var product in Link.Controller.products.all)
                {
                    if (product.definition.id == productId)
                    {
                        return product.metadata.localizedPriceString;
                    }
                }
            }

            return string.Empty;
        }
    }
}