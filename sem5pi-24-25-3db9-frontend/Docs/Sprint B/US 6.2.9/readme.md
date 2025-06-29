# US 6.2.9

## 1. Context

This task appears in the middle of the project's development, to be able to list patient profiles.


## 2. Requirements

**US 6.2.6** As an Admin, I want to list/search patient profiles by different attributes, so that I can view the details, edit, and remove patient profiles.

**Acceptance Criteria:**

- Admins can search patient profiles by various attributes, including name, email, date of birth,
or medical record number.
- The system displays search results in a list view with key patient information (name, email, date
of birth).
- Admins can select a profile from the list to view, edit, or delete the patient record.
- The search results are paginated, and filters are available to refine the search results.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin and that the patient profile is already in the system.
The backend component of this US must be already developed.

## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **

- **Question:**  
  - **Answer:** 



- **Question:** 
  - **Answer:** 


###  Domain Model excerpt
![DM Patient](DM%20Patient.png)

The following **HTTP requests** will be implemented:
- GET (with query parameters, to check specific patients)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US6.2.9 N1 SD](US6.2.9%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US6.2.9 N2 SD](US6.2.9%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US6.2.9 N3 SD](US6.2.9%20N3%20SD.png)





### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

The following tests are to be developed:
- all patient profiles must be displayed in the patient page.
- getting a patient profile by name, email, or dateOfBirth.

Unitary tests to the component will be implemented.

E2E Tests will also check all the US flow, from the user interaction to the backend server.

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #32 tag, as well as a relevant commit message.
The estimated duration for this US is 15h.

## 6. Integration/Demonstration

To list patient profiles, run the Backoffice, Auth and the SPA app, then go to the Patient page and check all shown patient profiles. Choose one profile to see the details. Choose a filter if desired.

## 7. Observations

This work was guided by the project provided in ARQSI classes.