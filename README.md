# **📌 Real-Time Chat Application - Backend**  
**Built with C#, ASP.NET Core, SignalR, and PostgreSQL**   

## **🔹 Overview**  
This is the **backend service** for a real-time chat application. It provides **authentication, user management, and message handling** through **ASP.NET Core and SignalR** for WebSocket-based communication. The system uses **PostgreSQL for data persistence** and enforces **secure API authentication** via cookies.  

## **🛠️ Tech Stack**  
- **Framework:** ASP.NET Core  
- **Real-Time Messaging:** SignalR  
- **Database:** PostgreSQL  
- **Authentication:** Cookie-based authentication  
- **Hosting:** Render  

## **✨ Features**  
✅ **Real-time WebSocket communication using SignalR**  
✅ **User authentication and session management**  
✅ **Channel-based messaging with access control**  
✅ **PostgreSQL-powered persistent message storage**  
✅ **Secure API endpoints with authentication middleware**  

## **📂 Setup Instructions**  

### **🔹 Prerequisites**  
- **.NET SDK 7.0+**  
- **PostgreSQL Database**  
- **Entity Framework Core**  

### **🔹 Installation & Setup**  
1. Clone the repository:  
   ```sh  
   git clone https://github.com/EljiahR/ChatProject.Reck.git  
   cd ChatProject.Reck  
   ```  
2. Configure the **PostgreSQL connection string** in `appsettings.json`.  
3. Apply database migrations:  
   ```sh  
   dotnet ef database update  
   ```  
4. Run the backend server:  
   ```sh  
   dotnet run  
   ```  

## **🌎 Deployment**  
- **Backend Hosted on Render:** [Insert Render Backend Link]  

## **🛠️ Future Improvements**  
- [ ] **Improve WebSocket connection handling for better scalability**  
- [ ] **Implement friend request system instead of instant friend-adding**  
- [ ] **Enhance security features (rate limiting, CSRF protection, etc.)**  
- [ ] **Add more robust logging and monitoring**  
