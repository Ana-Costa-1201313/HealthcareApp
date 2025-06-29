# US 7.2.4

## 1. Context

This task appears in the end of the project's development, to be able to create a medical condition.


## 2. Requirements

**US 7.2.4** As an Admin, I want to add new Medical Condition, so that the Doctors can use it to update the Patient Medical Record. 

**Acceptance Criteria:**

- Admins can medical condition details such as the name, code,longer description and a list of symptoms.
- The name and code are unique.
- The access is based on role-based permissions

**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:

```
Q: 
The medical condition consist in what?

Just a name or are there more fields?

A: 
it consists of a code (for example following ICD (International Classification of Diseases)), a designation and a longer description as well a list of common symptoms

```

```
Q: 
Earlier, you said the medical condition needed a code. 

Is this code automatic or is writen by the admin?

A: 
it must conform with the classficiation system you select, for instance, SNOMED CT (Systematized Nomenclature of Medicine - Clinical Terms) or ICD-11 (International Classification of Diseases, 11th Revision)

```

```
Q: 

Boa noite.
Qual seria o tamanho máximo de uma designação e descrição de uma alergia? 
Cumprimentos.



A: 
designação, max 100 caracteres
descrição, máximo 2048 caracteres

```

```
Q: 

Hello, 
What are the symptoms for a medical record? Are they a list already present in the system, and when creating the medical record, do you select the symptoms? If yes, what happens when a disease has a symptom that is not in that list? Who creates it in the system?
Thank you.

A: 
symptoms are free text

```


The following **HTTP requests** will be implemented:
- POST (to register the new medical condition)

## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Sequence Diagram (Level 1)

![SSD_Lvl1.png](SD1.svg) 


### 4.2. Sequence Diagram (Level 2)

![SSD_Lvl1.png](SD2.svg) 


### 4.3. Sequence Diagram (Level 3)

### 4.3.1 Post (Level 3)
This diagram guides the realization of the functionality, for level 3 process view, for both backend and frontend.

**Backend**

![USback](SD3Backend.svg)

**Frontend**
![USfront](SD3Frontend.svg)



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
- The name must be unique.
- The name is required.
- The code must be unique.
- The code is required.
- The description is required.

All Value Objects will be tested in Unitary Tests, to check if they fullfill their requirements.

The medical condition Service will be tested to see if the created medical condition is correct.

The medical condition Controller will be tested to see if the created medical condition and responses are correct.


## 5. Implementation

The implementation of this US is according to the design, as can be seen in the diagrams presented before.

All commits referred the corresponding issue in GitHub, using the #9 and #14 tag, as well as a relevant commit message.


## 6. Integration/Demonstration

To register a new medical condition, run the RecordsBackoffice app and AuthApp and send a POST HTTP request with the new data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.