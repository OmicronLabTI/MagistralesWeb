//
//  FileManager.swift
//  Omicron
//
//  Created by Axity on 27/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import  RxSwift
import RxCocoa

class FileManagerApp {
    static let shared: FileManagerApp = FileManagerApp()
    
    func saveSignatureOnIpad(signature: UIImage, name: String) -> Void {
        guard let documentsDirectory = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first else { return }

        let fileName = name
        let fileURL = documentsDirectory.appendingPathComponent(fileName)
        guard let data = signature.jpegData(compressionQuality: 1) else { return }

        //Checks if file exists, removes it if so.
        if FileManager.default.fileExists(atPath: fileURL.path) {
            do {
                try FileManager.default.removeItem(atPath: fileURL.path)
                print("Removed old image")
            } catch let removeError {
                print("couldn't remove file at path", removeError)
            }

        }

        do {
            try data.write(to: fileURL)
        } catch let error {
            print("error saving file with error", error)
        }
    }
    
    func getSignatureOnIpad(fileName: String) -> UIImage? {
        
       let documentDirectory = FileManager.SearchPathDirectory.documentDirectory

        let userDomainMask = FileManager.SearchPathDomainMask.userDomainMask
        let paths = NSSearchPathForDirectoriesInDomains(documentDirectory, userDomainMask, true)

        if let dirPath = paths.first {
            let imageUrl = URL(fileURLWithPath: dirPath).appendingPathComponent(fileName)
            let image = UIImage(contentsOfFile: imageUrl.path)
            return image

        }

        return nil
    }
    
    func deleteSignature(fileURL: URL) -> Void {
        do {
            try FileManager.default.removeItem(at: fileURL)
        } catch {
            print(error.localizedDescription)
        }
    }
}
