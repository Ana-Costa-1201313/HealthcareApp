# US 7.2.15

## 1. Context

This task appears in the end of the project's development, to be able to list specializations.


## 2. Requirements

**US 7.2.15** As an Admin, I want to list/search Specializations, so that I can see the details, and 
edit and remove Specializations. 

**Acceptance Criteria:**

- Admins can search specializations by attributes like: 
- Name 
- Code


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 


The following **HTTP requests** will be implemented:
- GET (to get specializations)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Sequence Diagram (Level 1)

![US7.2.15 N1 SD](US7.2.15%20N1%20SD.svg)


### 4.2. Sequence Diagram (Level 2)

![US7.2.15 N2 SD](US7.2.15%20N2%20SD.svg)


### 4.3. Sequence Diagram (Level 3)

#### 4.3.1. Backend

![US7.2.15 N3 SD](US7.2.15%20N3%20SD.svg)

#### 4.3.1. Frontend

![US7.2.15 N3 SPA](US7.2.15%20N3%20SPA.svg)

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

#### 4.5.1. Unit and Integration Tests

- Unit Tests for Specialization entity with valid inputs
- Unit Tests for Specialization entity with invalid inputs

- Unit tests for the controller focusing on the creation of the Specialization
- Unit tests for the service focusing on the creation of the Specialization
- Unit tests for the repository


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using and #25 tag, as well as a relevant commit message.



## 6. Integration/Demonstration

To get Specializations, run the Backoffice, Auth and SPA app then go to the Specialization page and click on the "eye" icon to get the details of a Specialization.

## 7. Observations

This work was guided by the project provided in ARQSI classes.