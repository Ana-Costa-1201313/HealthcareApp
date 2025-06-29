# US 5.1.10

## 1. Context

This task appears in the start of the project's development, to be able to delete a patient profile.


## 2. Requirements

**US 5.1.10** As an Admin, I want to delete a patient profile, so that I can remove patients who
are no longer under car

**Acceptance Criteria:**

- Admins can search for a patient profile and mark it for deletion.
- Before deletion, the system prompts the admin to confirm the action.
- Once deleted, all patient data is permanently removed from the system within a predefined time frame.
- The system logs the deletion for audit and GDPR compliance purposes.


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

- **

- **Question:** When generating the audit record to log the deletion of patient profiles what patient information (if any) are we allowed to keep in the log for identification purposes? If none are the logs then only a record of deletion operations and not actually tied to the deletion of a specific patient?

Best regards.
  - **Answer:** The ERS (health regulator) issued an opinion on the retention of health data in which it established a minimum retention period of 5 years, after which the data can be deleted. 

You may wish to keep some of the information anonymised for statistical purposes only, limiting yourself to, for example, gender and type of surgery.

You can find the issued opinion in moodle.


- **Question:** 
  - **Answer:** 


- **Question:** 
  - **Answer:** 


- **Question:** 
  - **Answer:** 


- **Question:**  
  - **Answer:** 



- **Question:** 
  - **Answer:** 



The following **HTTP requests** will be implemented:
- DELETE (to delete the new patient profile)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Level 1 Sequence Diagram

This diagram guides the realization of the functionality, for level 1 procecss view.

![US5.1.10 N1 SD](US5.1.10%20N1%20SD.png)


### 4.2. Level 2 Sequence Diagram

This diagram guides the realization of the functionality, for level 2 procecss view.

![US5.1.10 N2 SD](US5.1.10%20N2%20SD.png)


### 4.3. Level 3 Sequence Diagram

This diagram guides the realization of the functionality, for level 3 process view.

![US5.1.10 N3 SD](US5.1.10%20N3%20SD.png)




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

All commits referred the corresponding issue in GitHub, using the #11 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To delete a patient profile, run the Backoffice app and send a DELETE HTTP request with the new patient data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.