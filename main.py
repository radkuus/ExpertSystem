from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import json
import random
from typing import Any, Dict, Optional
from typing import List
import pandas as pd
import uuid
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.naive_bayes import GaussianNB
from sklearn.linear_model import LogisticRegression
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import f1_score, precision_score, recall_score, accuracy_score, confusion_matrix
import tensorflow
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Input, Dense

app = FastAPI()

class KNNDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float
    neighbors: int
    distance_metric: str
    user_samples: Optional[List[Dict[str, Any]]] = None   ## tylko jak uzytkownik da przyklady

class BayesDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float
    user_samples: Optional[List[Dict[str, Any]]] = None  

class NeuralNetworkDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    neurons: List[int]
    layers: int
    target_column: str
    training_size: float
    user_samples: Optional[List[Dict[str, Any]]] = None  

class IfThenDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    user_samples: Optional[List[Dict[str, Any]]] = None   
    ifthen: List[List[str]]

class LogisticRegressionInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float
    user_samples: Optional[List[Dict[str, Any]]] = None  

class ModelOutput(BaseModel):
    request_id: str
    f1: float
    precision: float
    recall: float
    accuracy: float
    confusion_matrix: List[List[int]]
    samples_history: List[str]


@app.get("/ServerStatus")
def server_status():
    return {"status": "available"}


def prepare_dataset(training_size, data, analysis_columns, target_column):
    
    full_columns = analysis_columns + [target_column]
    
    df = pd.DataFrame(data, columns=full_columns)

    
    for col in df.columns:
        if col != target_column:
            df[col] = pd.to_numeric(df[col], errors='coerce')

    df = df.dropna()
    X = df[analysis_columns]
    y = df[target_column]


    if training_size == 101: # specjalny warunek dla modelu ifthen
        X_test = df[analysis_columns]
        y_test = df[target_column]

        le = LabelEncoder()
        y_test = le.fit_transform(y_test)

        return X_test, y_test, le

    le = LabelEncoder()
    y = le.fit_transform(y)

    test_size = 1 - (training_size / 100)
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=test_size, random_state=42)

    return X_train, X_test, y_train, y_test, le

def prepare_user_samples(user_samples, X_train):
    ## teraz inputem jest json, więc po prostu przekształcam go na dataframe i zostawiam kolumny dgodne z X_train.columns (wybór uzytkownika)
    ## zmieniam przecinki na kropki 
    samples_df = pd.DataFrame(user_samples)
    samples_df = samples_df[X_train.columns]
    for col in samples_df.columns:
        samples_df[col] = samples_df[col].astype(str).str.replace(",", ".")
        samples_df[col] = pd.to_numeric(samples_df[col], errors="coerce")

    return samples_df



         
            
@app.post("/knn", response_model=ModelOutput)
async def run_knn(input_data: KNNDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test, le = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        if input_data.neighbors > len(X_train):
            raise HTTPException(status_code=400, detail = f"The number of neighbors ({input_data.neighbors}) cannot be greater than the number of rows in the training dataset ({len(X_train)}).")
        distance_metric = input_data.distance_metric.strip().lower()
        knn = KNeighborsClassifier(n_neighbors=input_data.neighbors, metric=distance_metric)
        knn.fit(X_train, y_train)
        y_pred = knn.predict(X_test)

        metrics = {
            "request_id": request_id,
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
            "samples_history": []           # domyslnie pusta lista
        }

        if input_data.user_samples:
            samples_df = prepare_user_samples(input_data.user_samples, X_train)
            prediction = knn.predict(samples_df)   
            prediction = le.inverse_transform(prediction)

            metrics["samples_history"] = [str(p) for p in prediction]


        print("KNN Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in KNN:", str(e))
        raise HTTPException(status_code=500, detail=str(e))



@app.post("/lr", response_model=ModelOutput)
async def run_lr(input_data: LogisticRegressionInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test, le = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        lr = LogisticRegression()
        lr.fit(X_train, y_train)
        y_pred = lr.predict(X_test)

        metrics = {
            "request_id": request_id,
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
            "samples_history": []
        }

        if input_data.user_samples:
            samples_df = prepare_user_samples(input_data.user_samples, X_train)
            prediction = lr.predict(samples_df)   
            prediction = le.inverse_transform(prediction)


            metrics["samples_history"] = [str(p) for p in prediction]

        print("LR Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in LR:", str(e))
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/bayes", response_model=ModelOutput)
async def run_bayes(input_data: BayesDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test, le = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        nb = GaussianNB()
        nb.fit(X_train, y_train)
        y_pred = nb.predict(X_test)

        metrics = {
            "request_id": request_id,
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
            "samples_history": []
        }

        if input_data.user_samples:
            samples_df = prepare_user_samples(input_data.user_samples, X_train)
            prediction = nb.predict(samples_df)   
            prediction = le.inverse_transform(prediction)   


            metrics["samples_history"] = [str(p) for p in prediction]

        print("Bayes Response:", metrics)
        
        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in Bayes:", str(e))
        raise HTTPException(status_code=500, detail=str(e))
    
@app.post("/NeuralNetwork", response_model=ModelOutput)
async def run_NeuralNetwork(input_data: NeuralNetworkDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test, le = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        model = Sequential()
        model.add(Input(shape=(X_train.shape[1],)))

        for i in range(input_data.layers):
            model.add(Dense(input_data.neurons[i], activation="relu"))
        
        num_classes = len(pd.unique(y_train))

        if num_classes == 2:
            model.add(Dense(1, activation="sigmoid"))
            loss = "binary_crossentropy"
        else:
            model.add(Dense(num_classes, activation="softmax"))
            loss = "sparse_categorical_crossentropy"
        
        model.compile(optimizer="adam", loss=loss, metrics=["accuracy"])
        model.fit(X_train, y_train, epochs=50, batch_size=32, verbose=0)
        
        y_pred_prob = model.predict(X_test)

        if num_classes == 2:
            y_pred = (y_pred_prob > 0.5).astype(int).flatten()
        else:
            y_pred = y_pred_prob.argmax(axis=1)

        metrics = {
            "request_id": request_id,
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
            "samples_history": []
        }


        if input_data.user_samples:
            samples_df = prepare_user_samples(input_data.user_samples, X_train)
            prediction_prob = model.predict(samples_df)

            if num_classes == 2:
                prediction = (prediction_prob > 0.5).astype(int).flatten()
            else:
                prediction = prediction_prob.argmax(axis=1)

            prediction = le.inverse_transform(prediction)
            metrics["samples_history"] = [str(p) for p in prediction]


        print("Neural Network Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in Neural Network:", str(e))
        raise HTTPException(status_code=500, detail=str(e))


def parse_rule(rule):
    conditions = []
    target_class = None

    i = 0
    while i < len(rule):
        token = rule[i]

        if token in ["If", "and"]:
            col = rule[i+1]
            op = rule[i+2]
            if "," in rule[i+3]:
                rule[i+3]=rule[i+3].replace(",",".")
            val = float(rule[i+3])
            conditions.append((col, op, val))
            i += 4
        elif token == "then":
            target_class = rule[i+1]
            i += 2
        else:
            i += 1

    return conditions, target_class

def apply_rules(rules, df, classes):
    ops = {
        ">": lambda a, b: a > b,
        "<": lambda a, b: a < b,
        "=": lambda a, b: a == b
    }
    predictions = []


    for _, row in df.iterrows():
        assigned = None
        for rule in rules:
            conditions, target_class = parse_rule(rule)

            if all(ops[op](row[col], val) for col, op, val in conditions):
                assigned = target_class
                break

        if assigned:
            predictions.append(assigned)
        else:
            predictions.append(random.choice(classes))

    return predictions


@app.post("/ifthen", response_model=ModelOutput)
async def run_IfThen(input_data: IfThenDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_test, y_test, le = prepare_dataset(101, input_data.data, input_data.analysis_columns, input_data.target_column)
        classes = le.inverse_transform(list(set(y_test)))
        cleaned = [] #usuwanie null 
        for row in input_data.ifthen:
            new_row = [item for item in row if item]
            cleaned.append(new_row)

        rules = []
        current_rule = []
        for row in cleaned:
            if row[0] == 'If':
                if current_rule:
                    rules.append(current_rule)
                current_rule = row
            elif row[0] in ('then', 'and'):
                current_rule.extend(row)
        if current_rule:
            rules.append(current_rule)
        
        print(rules)

        y_pred = apply_rules(rules, X_test, classes)
        y_pred = le.transform(y_pred)
        print(y_test,y_pred)

        metrics = {
            "request_id": request_id,
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "confusion_matrix": confusion_matrix(y_test, y_pred).tolist(),
            "samples_history": []
        }
        
        if input_data.user_samples:
            samples_df = prepare_user_samples(input_data.user_samples, X_test)
            prediction = apply_rules(rules, samples_df, classes)
            metrics["samples_history"] = [str(p) for p in prediction]

        print("IfThen Response:", metrics)
        
        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in IfThen:", str(e))
        raise HTTPException(status_code=500, detail=str(e))


