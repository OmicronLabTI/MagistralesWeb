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

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {
    
    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        // Override point for customization after application launch.
        self.setupRegistrationsDI()
        return true
    }

    // MARK: UISceneSession Lifecycle

    func application(_ application: UIApplication, configurationForConnecting connectingSceneSession: UISceneSession, options: UIScene.ConnectionOptions) -> UISceneConfiguration {
        // Called when a new scene session is being created.
        // Use this method to select a configuration to create the new scene with.
        return UISceneConfiguration(name: "Default Configuration", sessionRole: connectingSceneSession.role)
    }

    func application(_ application: UIApplication, didDiscardSceneSessions sceneSessions: Set<UISceneSession>) {
        // Called when the user discards a scene session.
        // If any sessions were discarded while the application was not running, this will be called shortly after application:didFinishLaunchingWithOptions.
        // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
    }
    
    func setupRegistrationsDI() {
        Resolver.register {
                    Config.isRunningTests ? NetworkManager(
                        provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub,plugins: [
                            AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.access_token })
                        ])) : NetworkManager()
                }.scope(Resolver.cached)
        Resolver.register { LoginViewModel() }
        Resolver.register { InboxViewModel() }.scope(Resolver.cached)
        Resolver.register { RootViewModel() }.scope(Resolver.cached)
        Resolver.register { OrderDetailViewModel()}.scope(Resolver.shared)
        Resolver.register { OrderDetailFormViewModel()}.scope(Resolver.shared)
        Resolver.register { CommentsViewModel() }.scope(Resolver.shared)
        Resolver.register { LotsViewModel() }.scope(Resolver.shared)
        Resolver.register { SignaturePadViewModel() }.scope(Resolver.shared)
        Resolver.register { LottieManager() }.scope(Resolver.shared)
        Resolver.register { ComponentsViewModel() }.scope(Resolver.shared)
        Resolver.register { ChartViewModel() }.scope(Resolver.shared)
    }
}
