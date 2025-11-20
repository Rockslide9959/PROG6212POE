# PROG6212 POE — Part 3 (Automation & Final Delivery)

This repository contains the final implementation for **Part 3** of the PROG6212 Portfolio of Evidence (POE). This part includes all automation features, HR functionality, lecturer improvements, role-based access control, a PowerPoint presentation, and an overview of changes made based on lecturer feedback.

---

## Project Overview

The **Contract Monthly Claim System (CMCS)** is an ASP.NET Core MVC web application that manages monthly claim submissions for Independent Contractor lecturers. Part 3 expands the system with automation, HR controls, enhanced validation, session-based security, and reporting.

---

## Part 3 Requirements Completed

Below is a detailed list of all Part 3 requirements and where they appear in this project.

### ### 1. HR View (Super User)

* HR can add users (Lecturers, Coordinators, Managers, HR).
* HR can update user information.
* HR can manage hourly rates.
* HR can generate reports using LINQ.
* HR can generate downloadable **PDF summaries** of claims.
* The system has **NO public registration page** — only HR creates accounts.

---

## ### 2. Lecturer View Enhancements

* Login required before accessing lecturer dashboard.
* Lecturer details (Name, Surname, HourlyRate) are automatically pulled from the database.
* **Auto-calculation** added: TotalAmount = HoursWorked × HourlyRate.
* Real-time calculation using JavaScript.
* Validation added for:

  * Maximum hours limit (180 hours).
  * Required document upload.
* Lecturer can track claim status through: `Pending → Verified → Approved / Rejected`.

---

## ### 3. Coordinator & Manager View Enhancements

* Session-based role restriction added.
* Coordinators can verify claims.
* Managers can approve or reject claims.
* Claims displayed in a structured, easy-to-read view.
* Role-specific pages protected so users cannot access unauthorized views.

---

## ### 4. System Automation

**Automation implemented across:**

* Lecturer calculation (auto total).
* Coordinator review workflow.
* Manager approval workflow.
* HR invoice/report generation.
* Auto-filling lecturer details.
* Auto-checking maximum hours.

---

## ### 5. Role-Based Access Control (Sessions)

Sessions used to restrict page access:

* Prevents unauthorized users from accessing pages.
* Ensures each user sees only their allowed dashboards.
* Session variables stored:

  * `UserId`
  * `UserRole`
  * `UserName`

---

## ### 6. Error Handling + Validation

* Full server-side validation using DataAnnotations.
* Client-side validation for hours and file upload.
* Meaningful error messages displayed.
* System handles missing documents, invalid login, and invalid claim submissions.

---

## ### 7. Reporting (HR)

HR view includes:

* LINQ-based monthly claim reports.
* Total hours and payments per lecturer.
* Optional PDF download.

---

## ### 8. Version Control

Minimum required commits: **10**
Repository includes:

* Continuous commits across development.
* Descriptive commit messages such as:

  * "Added HR role and user creation"
  * "Implemented auto-calculation for lecturer claim submission"
  * "Added session-based role restrictions"
  * "Integrated PDF report generator"
  * "Added Coordinator and Manager approval pages"

---

## ### 9. YouTube Video

A walkthrough video demonstrating the entire system is included here:
https://youtu.be/yRXkyQaFQNA

Video includes:

* Login for all roles.
* HR creating a user.
* Lecturer submitting claim with auto-calculation.
* Document upload demo.
* Coordinator verifying claim.
* Manager approving claim.
* HR generating PDF report.
* Session-based access blocking.

---

# System Structure

```
/Controllers
/Models
/Views
/wwwroot
/Reports
/Presentation
README.md
```

---

# How to Run the Project

1. Clone the repository.
2. Open in Visual Studio 2022.
3. Ensure SQL Server or LocalDB is running.
4. Update `appsettings.json` with your database connection string.
5. Run `Update-Database` (if migrations are used).
6. Run project.
7. Login using an HR account to add new users.

---

# Sample Login Credentials (for demo)

| Role        | Username     | Password |
| ----------- | ------------ | -------- |
| HR          | adminHR      | admin123 |
| Lecturer    | testLecturer | pass123  |
| Coordinator | testCoord    | pass123  |
| Manager     | testManager  | pass123  |


---

# Summary

Part 3 completes the CMCS system with:

* Full automation across all roles.
* HR as the central user manager.
* Secure session-based role control.
* Lecturer auto-calculation and validation.
* Coordinated approval workflow.
* Report generation.
* Fully documented and version-controlled repository.
* PowerPoint and demo video included.
