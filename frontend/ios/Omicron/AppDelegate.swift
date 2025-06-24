//
//  AppDelegate.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Resolver
import Moya
#if QA || RELEASE || UAT
import  Firebase
#endif

@UIApplicationMain
    class AppDelegate: UIResponder, UIApplicationDelegate {
    // swiftlint:disable line_length
    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        // Override point for customization after application launch.
        UIFont.overrideInitialize()
        self.setupRegistrationsDI()
        #if QA || RELEASE || UAT
        FirebaseApp.configure()
        #endif
        return true
    }

    // MARK: UISceneSession Lifecycle
    // swiftline:disable line_lenght
    func application(_ application: UIApplication, configurationForConnecting connectingSceneSession: UISceneSession, options: UIScene.ConnectionOptions) -> UISceneConfiguration {
        // Called when a new scene session is being created.
        // Use this method to select a configuration to create the new scene with.
        return UISceneConfiguration(name: "Default Configuration", sessionRole: connectingSceneSession.role)
    }

    func application(_ application: UIApplication, didDiscardSceneSessions sceneSessions: Set<UISceneSession>) {
    }
    func setupRegistrationsDI() {
        Resolver.register {
                    Config.isRunningTests ? NetworkManager(
                        provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub, plugins: [
                            AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.accessToken })
                        ])) : NetworkManager()
                }.scope(.cached)
        Resolver.register { LoginViewModel() }
        Resolver.register { InboxViewModel() }.scope(.cached)
        Resolver.register { RootViewModel() }.scope(.cached)
        Resolver.register { OrderDetailViewModel()}.scope(.shared)
        Resolver.register { OrderDetailFormViewModel()}.scope(.application)
        Resolver.register { CommentsViewModel() }.scope(.shared)
        Resolver.register { LotsViewModel() }.scope(.shared)
        Resolver.register { SignaturePadViewModel() }.scope(.shared)
        Resolver.register { LottieManager() }.scope(.shared)
        Resolver.register { SupplieViewModel() }.scope(.shared)
        Resolver.register { ComponentsViewModel() }.scope(.unique)
        Resolver.register { ChartViewModel() }.scope(.shared)
        Resolver.register { ContainerViewModel() }.scope(.unique)
        Resolver.register { BulkOrderViewModel() }.scope(.unique)
        Resolver.register { HistoryViewModel() }.scope(.application)
        Resolver.register { AddComponentViewModel() }.scope(.application)
    }
}
