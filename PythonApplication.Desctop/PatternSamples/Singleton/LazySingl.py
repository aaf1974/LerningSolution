#Беспонтовое решение, ен работает

class LazySingl:
    __instance = None
    __age = 50

    def __init__(self):
        if not LazySingl.__instance:
            print(" __init__ method called..")
        else:
            print("Instance already created:", self.getInstance())

        
    
    @classmethod
    def getInstance(cls):
        if not cls.__instance:
            cls.__instance = LazySingl()
        return cls.__instance


    @property
    def age(self):
        return self.__age
 
    @age.setter
    def age(self, age):
        self.__age = age
