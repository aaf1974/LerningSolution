#import ExampleBase
from ExampleBase import ExampleBase
from ChildClass import ExampleChild

#class ExampleUsing:
#    def __init__():
#        pass


class Person:
    def __init__(self, name, age):
        self.__name = name  # устанавливаем имя
        self.__age = age  # устанавливаем возраст
 
    @property
    def name(self):
        return self.__name
 
    @property
    def age(self):
        return self.__age
 
    @age.setter
    def age(self, age):
        if age in range(1, 100):
            self.__age = age
        else:
            print("Недопустимый возраст")
 
    def display_info(self):
        print("Имя:", self.__name, "\tВозраст:", self.__age)


def sampleUsing():
    baseInst = ExampleBase(111)
    baseInst.display_info()

    childInst = ExampleChild(3, 15)
    childInst.display_info()
    
    print("call toString()")
    print(baseInst)
    print(childInst)



if __name__ == "__main__":
    sampleUsing()