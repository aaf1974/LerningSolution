from tkinter import *
from tkinter.ttk import *
from tkinter import scrolledtext, filedialog, Menu
#from tkinter.ttk import Button
from tkinter import Menubutton
from os import path

#https://likegeeks.com/python-gui-examples-tkinter-tutorial/


def createWindowWithButton():
    window = Tk()
    window.title("Welcome to LikeGeeks app")
    window.geometry('550x300')

    txt = addEntry(window)
    label = addLabel(window)
    addButton(window, label, txt)
    

    addCombobox(window)
    addCheckbutton(window)
    addRadioButton(window)
    addScrolledtext(window)
    addSpinbox(window)
    addProgress(window)
    #addFileDialog(window)
    addMenu(window)


    window.mainloop()


def addEntry(window):
    txt = Entry(window, width=10)
    txt.grid(column=1, row=0)
    return txt

def addLabel(window):
    lbl = Label(window, text="Hello")
    #lbl = Label(window, text= 'label1', padx=5, pady=5) #not working
    lbl.grid(column=0, row=0)
    return lbl


def addButton(window, label, txt):
    def clicked():
        res = "Welcome to " + txt.get()
        label.configure(text= res)

    btn = Button(window, text="Click Me", command=clicked)
    btn.grid(column=2, row=0)


def addCombobox(window):
    combo = Combobox(window)
    combo['values']= (1, 2, 3, 4, 5, "Text")
    combo.current(1) #set the selected item
    combo.grid(column=0, row=1)



def addCheckbutton(window):
    chk_state = BooleanVar()
    chk_state.set(True) #set check state
    chk = Checkbutton(window, text='Choose', var=chk_state)
    chk.grid(column=0, row=2)



def addRadioButton(window):
    radioRowPlace = 3
    rad1 = Radiobutton(window,text='First', value=1)
    rad2 = Radiobutton(window,text='Second', value=2)
    rad3 = Radiobutton(window,text='Third', value=3)
    rad1.grid(column=0, row=radioRowPlace)
    rad2.grid(column=1, row=radioRowPlace)
    rad3.grid(column=2, row=radioRowPlace)



def addScrolledtext(window):
    txt = scrolledtext.ScrolledText(window, width=10, height=5)
    txt.grid(column=0,row=4)



def addSpinbox(window):
    spin = Spinbox(window, from_=0, to=100, width=5)
    spin.grid(column=0,row=5)


def addProgress(window):
    bar = Progressbar(window, length=200, style='black.Horizontal.TProgressbar')
    bar['value'] = 70
    bar.grid(column=1, row=5)


def addFileDialog(window):
    files = filedialog.askopenfilenames()
    file = filedialog.askopenfilename(filetypes = (("Text files","*.txt"),("all files","*.*")))
    dir = filedialog.askdirectory()
    file = filedialog.askopenfilename(initialdir= path.dirname(__file__))



def addMenu(window):

    def clicked():
        res = "menu item click "
        lbl.configure(text= res)

    menu = Menu(window)
    new_item = Menu(menu)
    new_item.add_command(label='New')
    new_item.add_separator()
    new_item.add_command(label='Edit')
    menu.add_cascade(label='File', menu=new_item)
    new_item = Menu(menu, tearoff=0)
    new_item.add_command(label='New', command=clicked)

    window.config(menu=menu)


if __name__ == "__main__":
    createWindowWithButton()

