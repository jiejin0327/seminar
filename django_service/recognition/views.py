from django.shortcuts import render
from django.http import HttpResponse
from pypinyin import pinyin, lazy_pinyin

import face_recognition
import cv2
import numpy as np
import pickle
import os
import json

# Create your views here.
def index(request):
    return HttpResponse("Hello!!!")

face_encodings = []
face_names = []
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
Template_DIR = os.path.join(BASE_DIR,'templates/rec.html').replace('\\','/')

def RecognitionImg(request,ImgPath=None):
        #識別--------------------
    name = "Unknown"

    try:

        #C:\KioskAi\RecognTemp
        #C:\KioskAi\RecognTemp
        FilePathDef = 'C:\KioskAi\ImageTable\RecognizeTemp'
        ImgPath = os.path.join(FilePathDef,os.path.basename(request.GET["ImgPath"]))
        #ImgPath = os.path.abspath(KeyVal)
    
        #ImgPath =
        #os.path.join(BASE_DIR,'recognition',ImgPath).replace('\\','/')

        #load training data
        f2 = open('TrainData.pkl', 'rb')
        known_face_names = pickle.load(f2)
        known_face_encodings = pickle.load(f2)
        f2.close()

        rgb_small_frame = face_recognition.load_image_file(ImgPath)
        face_encodings = face_recognition.face_encodings(rgb_small_frame)

        for face_encoding in face_encodings:
            # See if the face is a match for the known face(s)
            matches = face_recognition.compare_faces(known_face_encodings, face_encoding, tolerance=0.5)
    
            face_distances = face_recognition.face_distance(known_face_encodings, face_encoding)
            best_match_index = np.argmin(face_distances)
            if matches[best_match_index]:
                name = known_face_names[best_match_index]
            
            if name != "Unknown" :
                StarRec = DetectStar(ImgPath)
                msg = {"IsSuccess" : "True",
                      "Result" : json.dumps({"UserId" : name ,"StarId":StarRec}),
                      "ErrorMessage" : ""}
            else :
                msg = {"IsSuccess": "False",
                      "Result" : "",
                      "ErrorMessage" : "無法辨識此照片"}
               
    except Exception as e:
        msg = {"IsSuccess" : "False",
               "Result" : "",
               "ErrorMessage" : str(e)}

    ResultStr = json.dumps(msg)
    return HttpResponse(ResultStr)

def Training(request,UserId=None,ImgPath=None):
    FilePathDef = 'C:\KioskAi\ImageTable\ProfileImage'
    ImgPath = os.path.join(FilePathDef,request.GET["ImgPath"])

    print("訓練中")
    #TempName = ImgPath.split(".")[0].split('n/')[1]#分割字串
    TempName =request.GET["UserId"]

    try:
        # 檢查檔案是否存在
        if os.path.isfile('TrainData.pkl') :
            f2 = open('TrainData.pkl', 'rb')
            known_face_names = pickle.load(f2)
            known_face_encodings = pickle.load(f2)
            f2.close()

        else:
                known_face_names = []
                known_face_encodings = []
                f2 = open('TrainData.pkl', 'wb')
                pickle.dump(known_face_names, f2)
                pickle.dump(known_face_encodings, f2)
                f2.close()
    
        TempImg = face_recognition.load_image_file(ImgPath)
        TempEncoding = face_recognition.face_encodings(TempImg)[0]
        print(TempName + "訓練完成")

        # 將訓練資料存進list
        known_face_names.append(TempName)
        known_face_encodings.append(TempEncoding)


        # 將資料存進pickle
        f2 = open('TrainData.pkl', 'wb')
        pickle.dump(known_face_names, f2)
        pickle.dump(known_face_encodings, f2)
        f2.close()

        msg = {"IsSuccess" : "True",
               "Result" : json.dumps({"UserId" : TempName}),
               "ErrorMessage" : ""}
        
    except Exception as e:
        msg = {"IsSuccess" : "False",
               "Result" : "訓練失敗",
               "ErrorMessage" : str(e)}

    ResultStr = json.dumps(msg)
    return HttpResponse(ResultStr)
def DetectStar(ImgPath):
        #明星臉偵測--------------------
    name = "Unknown"
    
    try:

        FilePathDef = 'C:\KioskAi\ImageTable\RecognizeTemp'
        #ImgPath = os.path.join(FilePathDef,os.path.basename(request.GET["ImgPath"]))

        #load training data
        f2 = open('TrainData_Star.pkl', 'rb')
        known_face_names = pickle.load(f2)
        known_face_encodings = pickle.load(f2)
        f2.close()

        rgb_small_frame = face_recognition.load_image_file(ImgPath)
        face_encodings = face_recognition.face_encodings(rgb_small_frame)

        for face_encoding in face_encodings:
            # See if the face is a match for the known face(s)
            matches = face_recognition.compare_faces(known_face_encodings, face_encoding, tolerance=1)
    
            face_distances = face_recognition.face_distance(known_face_encodings, face_encoding)
            best_match_index = np.argmin(face_distances)
            if matches[best_match_index]:
                name = known_face_names[best_match_index]


            if name != "Unknown" :
                return name

            else :
                msg = {"StarIsSuccess": "False",
                      "StarResult" : "",
                      "StarErrorMessage" : "無法辨識此照片"}
               
    except Exception as e:
        msg = {"StarIsSuccess" : "False",
               "StarResult" : "",
               "StarErrorMessage" : str(e)}

    #ResultStr = json.dumps(msg)
    #return HttpResponse(ResultStr)

