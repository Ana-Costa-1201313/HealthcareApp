# US 5.1.9

## 1. Context

This task appears in the start of the project's development, to be able to edit a patient profile.


## 2. Requirements

**US 5.1.9** As an Admin, I want to edit an existing patient profile, so that I can update their
information when needed.

**Acceptance Criteria:**

- Admins can search for and select a patient profile to edit.
- Editable fields include name, contact information, medical history, and allergies.
- Changes to sensitive data (e.g., contact information) trigger an email notification to the patient.
- The system logs all profile changes for auditing purposes.

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **

- **Question:** In this US an admin can edit a user profile. Does the system display a list of all users or the admin searchs by ID? Or both?
  - **Answer:** this requirement is for the editing of the user profile. from a usability point of view, the user should be able to start this feature either by searching for a specific user or listing all users and selecting one.
note that we are not doing the user interface of the system in this sprint.


- **Question:** Regarding the editing of patient information, is contact information the only sensitive data? Is it the only data that triggers an email notification?
  - **Answer:** faz parte das vossas responsabilidades no âmbito do módulo de proteçãod e dados e de acordo com a politica que venham a definir


- **Question:** 
  - **Answer:** 


- **Question:** 
  - **Answer:** 


- **Question:**  
  - **Answer:** 



- **Question:** 
  - **Answer:** 



The following **HTTP requests** will be implemented:
- PATCH (to edit the new patient profile)
- PUT (to edit the new patient profile)
## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US5.1.9 N1 SD](US5.1.9%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US5.1.9 N2 SD](US5.1.9%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US5.1.9 N3 SD PATCH](US5.1.9%20N3%20SD%20PATCH.png)
![US5.1.9 N3 SD PUT](US5.1.9%20N3%20SD%20PUT.png)



### 4.5. Applied Design Patterns

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

The following tests were developed:
- Controller Tests
- Service Tests
- Result, Error/Sucess code


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #10 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To edit a new patient profile, run the Backoffice app and send a PATCH or PUT HTTP request with the new patient data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.