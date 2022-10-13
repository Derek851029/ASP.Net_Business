# -*- coding: utf-8 -*-
"""
Created on Wed Oct 28 10:13:09 2020

"""
import pyodbc
import datetime, time, threading
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.image import MIMEImage
from pathlib import Path
import smtplib

def snedmail(tomail,contact, bus_en,newsarray):
    time.sleep(5)
    blank = "<br>"
    content = MIMEMultipart()  #建立MIMEMultipart物件
    content["subject"] = "為您搜尋有關您公司的資料"  #郵件標題
    content["from"] = "acrm@phrs.com.tw"  #寄件者
    content["to"] = tomail #收件者
    content.attach(MIMEText(""+contact+""+bus_en+"您好: "+blank+"以下是我這段時間為您蒐集之產業相關資料，希望對您有幫助"+blank+""+newsarray+"",'html'))  #郵件內容

    with smtplib.SMTP(host="smtp.gmail.com", port="587") as smtp:  # 設定SMTP伺服器
        try:
            smtp.ehlo()  # 驗證SMTP伺服器
            smtp.starttls()  # 建立加密傳輸
            smtp.login("acrm@phrs.com.tw", "12345678")  # 登入寄件者gmail
            smtp.send_message(content)  # 寄送郵件
            print("Complete!")
        except Exception as e:
            print("Error message: ", e)
        

def findemail(findemail_sql):
    conn = pyodbc.connect('DRIVER={SQL Server}; SERVER=DESKTOP-UT2ARF9,1433\MSSQLSERVER2012; DATABASE=Customer; UID=sa; PWD=Acme-70472615') 

    cursor = conn.cursor()
    
    sqlstr = findemail_sql
    
    cursor.execute(sqlstr)
    
    email = []
    
    for i in cursor:
        contact = []
        for j in i:
            contact.append(j)
        email.append(contact)
            
    conn.close()
    return email

def findtodayweek(todayweek):
    conn = pyodbc.connect('DRIVER={SQL Server}; SERVER=DESKTOP-UT2ARF9,1433\MSSQLSERVER2012; DATABASE=Customer; UID=sa; PWD=Acme-70472615') 

    cursor = conn.cursor()
    
    sqlstr = todayweek
    
    cursor.execute(sqlstr)
    
    week = []
    
    for i in cursor:
        for j in i:
            week.append(j)
            
    conn.close()
    return week

def findsendnews(keyword, Owner):
    conn = pyodbc.connect('DRIVER={SQL Server}; SERVER=DESKTOP-UT2ARF9,1433\MSSQLSERVER2012; DATABASE=Customer; UID=sa; PWD=Acme-70472615') 

    cursor = conn.cursor()
    
    sqlstr = "SELECT href,title FROM AutoSendSearch WHERE bs_Owner='"+keyword+"' AND Owner='"+Owner+"' AND Type='已加入'"
    
    cursor.execute(sqlstr)

    newsarray = ""
    for i in cursor:
        href = "<a href='{}'>{}</a>"
        newsarray += href.format(i[0],i[1])
        newsarray += "<br>"
        setflag(i[0], i[1], keyword, Owner)
    conn.close()
    return newsarray

def setflag(href,title,keyword, Owner):
    conn = pyodbc.connect('DRIVER={SQL Server}; SERVER=DESKTOP-UT2ARF9,1433\MSSQLSERVER2012; DATABASE=Customer; UID=sa; PWD=Acme-70472615') 

    cursor = conn.cursor()
    
    sqlstr = "UPDATE [dbo].[AutoSendSearch] SET Type='已寄出' WHERE title = '"+title+"' AND href='"+href+"' AND Owner = '"+Owner+"' AND bs_Owner='"+keyword+"'"
    cursor.execute(sqlstr)
    conn.commit()

def controller(): #email陣列 array=每周 array2=兩周 array3=一個月    
    todayweek = findtodayweek("SELECT DATENAME(DW,GETDATE())")[0]
    today = datetime.datetime.now()
    todaydate = today.day
    try:
        emailarray = findemail("SELECT b.EMAIL, b.CONTACT, b.BUSINESSNAME_EN, b.BUSINESSNAME, b.Owner from DispatchSystem a left join BusinessData b ON cast(a.SYSID as nvarchar(max)) = b.Owner left join SendNewSchedule c ON b.BUSINESSNAME = c.businessname and cast(a.SYSID as nvarchar(max)) = c.Owner WHERE b.Type = '保留' AND a.Agent_Status='在職' AND c.cycle = '每周' AND c.day = '"+todayweek+"'")
        for i in emailarray:
            newsarray = findsendnews(i[3],i[4]) #3是新聞關鍵字 4是Owner
            if len(newsarray) == 0:
                continue
            if i[0] == "":
                continue
            snedmail(i[0],i[1], i[2], newsarray) #0是email, 1是聯絡人, 2是職稱
        emailarray2 = findemail("SELECT b.EMAIL, b.CONTACT, b.BUSINESSNAME_EN, b.BUSINESSNAME, b.Owner from DispatchSystem a left join BusinessData b ON cast(a.SYSID as nvarchar(max)) = b.Owner left join SendNewSchedule c ON b.BUSINESSNAME = c.businessname and cast(a.SYSID as nvarchar(max)) = c.Owner WHERE b.Type = '保留' AND a.Agent_Status='在職' AND c.cycle = '兩周' AND c.day = '"+todayweek+"'")
        for x in emailarray2:
            newsarray = findsendnews(x[3],x[4])
            if len(newsarray) == 0:
                continue
            if x[0] == "":
                continue
            snedmail(x[0],x[1], x[2], newsarray)
        emailarray3 = findemail("SELECT b.EMAIL, b.CONTACT, b.BUSINESSNAME_EN, b.BUSINESSNAME, b.Owner from DispatchSystem a left join BusinessData b ON cast(a.SYSID as nvarchar(max)) = b.Owner left join SendNewSchedule c ON b.BUSINESSNAME = c.businessname and cast(a.SYSID as nvarchar(max)) = c.Owner WHERE b.Type = '保留' AND a.Agent_Status='在職' AND c.cycle = '一個月' AND c.day = '"+str(todaydate)+"'")
        for y in emailarray3:
            newsarray = findsendnews(y[3],y[4])
            if len(newsarray) == 0:
                continue
            if y[0] == "":
                continue
            snedmail(y[0],y[1], y[2], newsarray)
    except Exception as e:
        print(e)
if __name__ == '__main__':
    controller()