//
//  Persistence.swift
//  Omicron
//
//  Created by Diego Cárcamo on 04/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

class Persistence {
    //MARK: Variables
    static let shared: Persistence = Persistence()
    let userDefaults = UserDefaults.standard
    //MARK: Functions
    func saveLoginData(data: LoginResponse) {
        do {
            try self.userDefaults.setObject(data, forKey: UsersDefaultsConstants.loginData)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    func getLoginData() -> LoginResponse? {
        do {
            let data = try self.userDefaults.getObject(forKey: UsersDefaultsConstants.loginData, castTo: LoginResponse.self)
            return data
        } catch {
            print(error.localizedDescription)
            return nil
        }
    }
    
    func saveUserName(username: String) {
        do {
            try self.userDefaults.setObject(username, forKey: UsersDefaultsConstants.username)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    func getUserName() -> String {
        do {
            let data = try self.userDefaults.getObject(forKey: UsersDefaultsConstants.username, castTo: String.self)
            return data
        } catch {
            print(error.localizedDescription)
            return ""
        }
    }
    
    func saveUserData(user: User) -> Void {
        do {
            try self.userDefaults.setObject(user, forKey: UsersDefaultsConstants.userData)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    func getUserData() -> User? {
        do {
            let data = try self.userDefaults.getObject(forKey: UsersDefaultsConstants.userData, castTo: User.self)
            return data
        } catch {
            print(error.localizedDescription)
            return nil
        }
    }
    
    func getIsLogged() -> Bool {
        
        let data = self.userDefaults.bool(forKey: UsersDefaultsConstants.isLogged)
        return data
    }
    
    func saveIsLogged(isLogged: Bool) -> Void {
        self.userDefaults.set(isLogged, forKey: UsersDefaultsConstants.isLogged)
    }
    
    func removePersistenceData() -> Void {
        let domain = Bundle.main.bundleIdentifier!
        UserDefaults.standard.removePersistentDomain(forName: domain)
        UserDefaults.standard.synchronize()
    }
    
}

protocol ObjectSavable {
    func setObject<Object>(_ object: Object, forKey: String) throws where Object: Encodable
    func getObject<Object>(forKey: String, castTo type: Object.Type) throws -> Object where Object: Decodable
}

enum ObjectSavableError: String, LocalizedError {
    case unableToEncode = "Unable to encode object into data"
    case noValue = "No data object found for the given key"
    case unableToDecode = "Unable to decode object into given type"
    
    var errorDescription: String? {
        rawValue
    }
}

extension UserDefaults: ObjectSavable {
    func setObject<Object>(_ object: Object, forKey: String) throws where Object: Encodable {
        let encoder = JSONEncoder()
        do {
            let data = try encoder.encode(object)
            set(data, forKey: forKey)
        } catch {
            throw ObjectSavableError.unableToEncode
        }
    }
    
    func getObject<Object>(forKey: String, castTo type: Object.Type) throws -> Object where Object: Decodable {
        guard let data = data(forKey: forKey) else { throw ObjectSavableError.noValue }
        let decoder = JSONDecoder()
        do {
            let object = try decoder.decode(type, from: data)
            return object
        } catch {
            throw ObjectSavableError.unableToDecode
        }
    }
}
