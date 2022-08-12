import sys
import traceback
import FaceDetection.collect_faces
print("python已啟動")

#while True:
#    InputVal = input()
#    if InputVal.isnumeric():
#        print(int(InputVal) + 100)
#    else:
#        print(InputVal)

#try:
#    collect_faces.run()
#except Exception as e:
#    print(e)
#    error_class = e.__class__.__name__ #取得錯誤類型
#    detail = e.args[0] #取得詳細內容
#    cl, exc, tb = sys.exc_info() #取得Call Stack
#    lastCallStack = traceback.extract_tb(tb)[-1] #取得Call Stack的最後一筆資料
#    fileName = lastCallStack[0] #取得發生的檔案名稱
#    lineNum = lastCallStack[1] #取得發生的行號
#    funcName = lastCallStack[2] #取得發生的函數名稱
#    errMsg = "File \"{}\", line {}, in {}: [{}] {}".format(fileName, lineNum, funcName, error_class, detail)
#    print(errMsg)
