# US 5.1.11

## 1. Context

This task appears in the start of the project's development, to be able to search a list of patients by various attributes.


## 2. Requirements

**US 5.1.10** As an Admin, I want to list/search patient profiles by different attributes, so that I
can view the details, edit, and remove patient profiles

**Acceptance Criteria:**

- Admins can search patient profiles by various attributes, including name, email, date of birth,
or medical record number.
- The system displays search results in a list view with key patient information (name, email, date
of birth).
- Admins can select a profile from the list to view, edit, or delete the patient record.
- The search results are paginated, and filters are available to refine the search results.


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **

- **Question:**
  - **Answer:** 


- **Question:** 
  - **Answer:** 



The following **HTTP requests** will be implemented:
- GET By Various Attributes

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US5.1.11 N1 SD](US5.1.11%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US5.1.11 N2 SD](US5.1.11%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US5.1.11 N3 SD](US5.1.11%20N3%20SD.png)




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
The following tests were developed:
- Controller Tests
- Service Tests
- Result, Error/Sucess code


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #12 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To search a list of patients by various atttributes, run the Backoffice app and send a GET/ByVariousAttributes HTTP request with the attributes that you want to search.

## 7. Observations

This work was guided by the project provided in ARQSI classes.